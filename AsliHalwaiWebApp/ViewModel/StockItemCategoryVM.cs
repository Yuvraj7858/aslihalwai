using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockItemCategoryVM
    {
        public int StockItemCategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string StockItemCategory { get; set; }

        [Required(ErrorMessage = "Visible Status is required")]
        public int VisibleStatusId { get; set; }

        // Dropdown
        public List<SelectListItem> VisibleStatusList { get; set; } = new();

        // List
        public List<StockItemCategoryVM> CategoryList { get; set; } = new();
    }
}
