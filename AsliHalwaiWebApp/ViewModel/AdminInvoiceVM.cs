namespace AsliHalwaiWebApp.ViewModel
{
    public class AdminInvoiceVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; } = "";
        public string ContactNo { get; set; } = "";

        public string Address { get; set; }

        public string? PaymentMode { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentUTR { get; set; }
        public string OrderStatus { get; set; } = "";
        public decimal TotalAmount { get; set; }
    }
}
