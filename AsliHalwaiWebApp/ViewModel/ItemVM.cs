namespace AsliHalwaiWebApp.Models
{
    public class ItemVM
    {
        public int StockItemID { get; set; }
        public string? StockItemCategory { get; set; }
        public string? ItemPhotoPath { get; set; } = string.Empty;
        public string? StockItemTitle { get; set; } = string.Empty;
        public string? ItemSize { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public DateTime RegisterDate { get; set; }
        public string? VisibleStatus { get; set; }
        public string? CreatedBy { get; set; }
        public string? OrderTypeName { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
