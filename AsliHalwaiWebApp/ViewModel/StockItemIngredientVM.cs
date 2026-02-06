using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockItemIngredientVM
    {
        // ===== ADD / EDIT =====
        public int StockItemIngredientId { get; set; }

        public int StockItemId { get; set; }

        [Required(ErrorMessage = "Ingredient name required")]
        public string StockItemIngredientTitle { get; set; }

        public IFormFile IngredientPhoto { get; set; }
        public string IngredientPhotoPath { get; set; }

        // ===== DROPDOWN =====
        public List<SelectListItem> StockItemList { get; set; } = new();

        // ===== LIST =====
        public List<StockItemIngredientListVM> IngredientList { get; set; } = new();
    }

    public class StockItemIngredientListVM
    {
        public int StockItemIngredientId { get; set; }
        public string StockItemIngredientTitle { get; set; }
        public string IngredientPhotoPath { get; set; }
        public string StockItemTitle { get; set; }
        public string CreatedBy { get; set; }
    }
}
