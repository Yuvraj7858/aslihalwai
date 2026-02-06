using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockMenuCategoryVM
    {
        // Add / Edit
        public int StockMenuCategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string StockMenuCategory { get; set; }

        // List
        public List<StockMenuCategoryListVM> CategoryList { get; set; } = new();
    }

    public class StockMenuCategoryListVM
    {
        public int StockMenuCategoryId { get; set; }
        public string StockMenuCategory { get; set; }

        public string CreatedBy { get; set; }
    }
}
