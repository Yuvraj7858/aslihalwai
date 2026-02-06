namespace AsliHalwaiWebApp.ViewModel
{
    public class CustomerStockItemVM
    {
        public int StockItemId { get; set; }
        public string StockItemTitle { get; set; }
        public string ItemPhotoPath { get; set; }
        public string ItemSize { get; set; }
        public double UnitPrice { get; set; }
        public string OrderType { get; set; }
        public string VisibleStatus { get; set; }
    }
}
