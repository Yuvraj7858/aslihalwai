using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockItemVM
    {
        // ===== ADD / EDIT =====
        public int StockItemId { get; set; }

        [Required]
        public string StockItemTitle { get; set; }

        public string ItemSize { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        public IFormFile? ItemPhoto { get; set; }
        public string? ItemPhotoPath { get; set; }

        [Required]
        public int StockItemCategoryId { get; set; }

        [Required]
        public int OrderTypeId { get; set; }

        [Required]
        public int VisibleStatusId { get; set; }

        // ===== DROPDOWNS =====
        public List<SelectListItem> StockItemCategoryList { get; set; } = new();
        public List<SelectListItem> OrderTypeList { get; set; } = new();
        public List<SelectListItem> VisibleStatusList { get; set; } = new();

        // ===== LIST =====
        public List<StockItemListVM> ItemList { get; set; } = new();
    }

    public class StockItemListVM
    {
        public int StockItemId { get; set; }
        public string StockItemTitle { get; set; }
        public string StockItemCategory { get; set; }
        public string OrderType { get; set; }
        public string ItemSize { get; set; }
        public double UnitPrice { get; set; }
        public string VisibleStatus { get; set; }
        public string CreatedBy { get; set; }
        public string ItemPhotoPath { get; set; }
    }
}
