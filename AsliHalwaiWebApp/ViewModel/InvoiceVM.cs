namespace AsliHalwaiWebApp.ViewModel
{
    public class InvoiceVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; } = "";
        public string ContactNo { get; set; } = "";
        public string Address { get; set; } = "";

        public string OrderStatus { get; set; } = "";

        public List<InvoiceItemVM> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
    }

    public class InvoiceItemVM
    {
        public string ProductName { get; set; } = "";
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Qty * UnitPrice;
    }
}
