using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockDealItemVM
    {
        // ===== ADD / EDIT =====
        public int StockDealDetailId { get; set; }
        public int StockDealId { get; set; }

        [Required]
        public int StockItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public double Discount { get; set; }

        [Required]
        public int VisibleStatusId { get; set; }

        // ===== DROPDOWNS =====
        public List<SelectListItem> StockItemList { get; set; } = new();
        public List<SelectListItem> VisibleStatusList { get; set; } = new();

        // ===== LIST =====
        public List<StockDealItemListVM> DealItems { get; set; } = new();
    }

    public class StockDealItemListVM
    {
        public int StockDealDetailId { get; set; }
        public int StockDealId { get; set; }

        public string StockItemTitle { get; set; }
        public string ItemPhotoPath { get; set; }
        public string ItemSize { get; set; }
        public string OrderType { get; set; }

        public int Quantity { get; set; }
        public double Discount { get; set; }
        public string VisibleStatus { get; set; }
    }
}
