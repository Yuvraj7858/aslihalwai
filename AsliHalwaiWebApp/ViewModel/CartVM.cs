namespace AsliHalwaiWebApp.ViewModel
{
    public class CartVM
    {
        public int CartId { get; set; }
        public int CartItemId { get; set; }

        public int StockItemId { get; set; }
        public string StockItemTitle { get; set; }
        public string ItemPhotoPath { get; set; }
        public string ItemSize { get; set; }

        // 🔥 NEW (tutorial ke according)
        public string StockItemCategory { get; set; }

        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }

        public decimal ItemCost => Qty * UnitPrice;

        // "item" / "deal"
        public string ItemType { get; set; }
    }
}
