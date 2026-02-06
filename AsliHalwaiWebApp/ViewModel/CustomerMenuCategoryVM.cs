using System.Collections.Generic;

namespace AsliHalwaiWebApp.ViewModel
{
    public class CustomerMenuCategoryVM
    {
        public int StockItemCategoryId { get; set; }
        public string StockCategory { get; set; }

        public List<CustomerStockItemVM> Items { get; set; } = new();
    }
}
