using System.Collections.Generic;
using Graduation_Project.Models;

namespace Graduation_Project.ViewModels
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; }
        public List<UserAddress> Addresses { get; set; }
        public int? SelectedAddressId { get; set; }
    }
}
