namespace AsliHalwaiWebApp.ViewModel
{
    public class UpiPaymentVM
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }

        public string UpiId { get; set; } = "aslihalwai@upi";
        public string MerchantName { get; set; } = "Asli Halwai";

        public string? PaymentUTR { get; set; }
    }

}
