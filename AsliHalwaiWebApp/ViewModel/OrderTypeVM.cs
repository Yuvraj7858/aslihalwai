using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class OrderTypeVM
    {
        public int OrderTypeId { get; set; }

        [Required(ErrorMessage = "Order Type is required")]
        public string OrderTypeName { get; set; } = null!;

        // List for table
        public List<OrderTypeVM> OrderTypeList { get; set; } = new();
    }
}
