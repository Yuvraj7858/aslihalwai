using AsliHalwaiWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AsliHalwaiWebApp.ViewModel
{
    public class ShoppingCartVM
    {
        // CART
        public List<CartVM> CartItems { get; set; } = new();
        public string PaymentMode { get; set; } = "COD";
        public string? PaymentUTR { get; set; }

        // FORM FIELDS
        public int OrderTypeId { get; set; }
        public int UserAddressId { get; set; }

        public OrderTable Order { get; set; } = new OrderTable();
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string ContactNo { get; set; } = "";

        public bool CashOnDelivery { get; set; }

        // 🔥 REQUIRED FOR DROPDOWNS (THIS WAS MISSING)
        public List<SelectListItem> OrderTypes { get; set; } = new();
        public List<SelectListItem> Addresses { get; set; } = new();

        // ctor
        public ShoppingCartVM() { }

        public ShoppingCartVM(string firstName, string lastName, string contactNo)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactNo = contactNo;
        }
    }
}
