using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            if (ModelState.IsValid)
            {
                address.UserId = userId;

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

                _context.Add(address);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Address added successfully.";
                return RedirectToAction(nameof(Index));
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

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Address updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.AddressId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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

