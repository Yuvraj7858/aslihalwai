using System.Collections.Generic;

namespace AsliHalwaiWebApp.ViewModel
{
    public class HomeVM
    {
        // MENU (Category wise items)
        public List<MenuCategoryVM> Menu { get; set; } = new();

        // POPULAR ITEMS
        public List<HomeStockItemVM> PopularDishes { get; set; } = new();
    }

    // 🔹 Category + Items
    public class MenuCategoryVM
    {
        public int StockMenuCategoryId { get; set; }
        public string MenuCategory { get; set; }

        public List<HomeStockItemVM> Lists { get; set; } = new();
    }

    // 🔹 Item VM (Customer View)
    public class HomeStockItemVM
    {
        public int StockItemId { get; set; }
        public string StockItemTitle { get; set; }
        public string ItemSize { get; set; }
        public double UnitPrice { get; set; }
        public string ItemPhotoPath { get; set; }
        public string OrderType { get; set; }
        public string VisibleStatus { get; set; }
    }
}
