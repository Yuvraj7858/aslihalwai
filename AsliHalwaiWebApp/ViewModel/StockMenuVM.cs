using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModels;

namespace AsliHalwaiWebApp.ViewModel
{
    public class StockMenuVM
    {

        public List<StockCategoryVM> Categories { get; set; } = new();

        public string? SearchKey { get; set; }

        public List<ItemVM> AllItems { get; set; } = new();
        public List<FilterCategoryVM> CategoryList { get; set; } = new();
        public List<FilterOrderTypeVM> OrderTypeList { get; set; } = new();
    }
}
