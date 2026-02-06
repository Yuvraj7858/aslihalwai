using System.Collections.Generic;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockCategoryVM
    {
        public int StockItemCategoryId { get; set; }
        public string StockCategory { get; set; }

        public List<StockItemListVM> Items { get; set; } = new();
    }
}
