using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Graduation_Project.Controllers
{
    public class AddressController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressController(GraduationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Address
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.FullName)
                .ToListAsync();

            // Success message is handled via TempData in the view
            
            return View(addresses);
        }

        // GET: Address/Create
        public IActionResult Create()
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.UserAddress address)
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Set the UserId before validation
            address.UserId = userId;
            
            // Clear validation errors for NotMapped properties
            ModelState.Remove("User");
            ModelState.Remove("AddressName");
            ModelState.Remove("Phone");
            ModelState.Remove("AddressType");
            ModelState.Remove("DeliveryInstructions");

            if (ModelState.IsValid)
            {
                try
                {
                    // If this is the first address or marked as default
                    if (address.IsDefault || !_context.UserAddresses.Any(a => a.UserId == userId))
                    {
                        // Set all other addresses as non-default
                        var existingAddresses = await _context.UserAddresses
                            .Where(a => a.UserId == userId)
                            .ToListAsync();

                        foreach (var existingAddress in existingAddresses)
                        {
                            existingAddress.IsDefault = false;
                        }

                        address.IsDefault = true;
                    }

                    // If the NotMapped Phone property is provided, use it for PhoneNumber
                    if (!string.IsNullOrEmpty(address.Phone) && string.IsNullOrEmpty(address.PhoneNumber))
                    {
                        address.PhoneNumber = address.Phone;
                    }

                    _context.Add(address);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Address added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your address. Please try again.");
                    TempData["ErrorMessage"] = "Failed to save address. Please try again.";
                }
            }
            else
            {
                // Log validation errors for debugging
                var errorMessages = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }
                
                if (errorMessages.Any())
                {
                    TempData["ErrorMessage"] = string.Join(", ", errorMessages);
                }
            }
            return View(address);
        }

        // GET: Address/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.UserAddress address)
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != address.AddressId || userId != address.UserId)
            {
                return NotFound();
            }
            
            // Clear validation errors for NotMapped properties
            ModelState.Remove("User");
            ModelState.Remove("AddressName");
            ModelState.Remove("Phone");
            ModelState.Remove("AddressType");
            ModelState.Remove("DeliveryInstructions");

            if (ModelState.IsValid)
            {
                try
                {
                    // If this address is being set as default
                    if (address.IsDefault)
                    {
                        // Set all other addresses as non-default
                        var existingAddresses = await _context.UserAddresses
                            .Where(a => a.UserId == userId && a.AddressId != id)
                            .ToListAsync();

                        foreach (var existingAddress in existingAddresses)
                        {
                            existingAddress.IsDefault = false;
                        }
                    }
                    else
                    {
                        // Check if this is the only default address
                        var isOnlyDefault = await _context.UserAddresses
                            .Where(a => a.UserId == userId && a.IsDefault && a.AddressId == id)
                            .AnyAsync();

                        // If this is the only default address, keep it as default
                        if (isOnlyDefault && !await _context.UserAddresses.AnyAsync(a => a.UserId == userId && a.IsDefault && a.AddressId != id))
                        {
                            address.IsDefault = true;
                        }
                    }

                    // If the NotMapped Phone property is provided, use it for PhoneNumber
                    if (!string.IsNullOrEmpty(address.Phone) && string.IsNullOrEmpty(address.PhoneNumber))
                    {
                        address.PhoneNumber = address.Phone;
                    }

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Address updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!AddressExists(address.AddressId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The address was modified by another user. Please try again.";
                        return View(address);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating your address. Please try again.");
                    TempData["ErrorMessage"] = "Failed to update address. Please try again.";
                    return View(address);
                }
            }
            else
            {
                // Log validation errors for debugging
                var errorMessages = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }
                
                if (errorMessages.Any())
                {
                    TempData["ErrorMessage"] = string.Join(", ", errorMessages);
                }
            }
            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            // If deleting a default address, set another address as default if available
            if (address.IsDefault)
            {
                var nextAddress = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.AddressId != id)
                    .FirstOrDefaultAsync();

                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                }
            }

            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Address deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete address. Please try again.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Address/SetDefault/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            // Check if user is logged in
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            // Set all addresses as non-default
            var allAddresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var addr in allAddresses)
            {
                addr.IsDefault = (addr.AddressId == id);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Default address updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(int id)
        {
            return _context.UserAddresses.Any(e => e.AddressId == id);
        }
    }
}



