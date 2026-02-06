namespace AsliHalwaiWebApp.ViewModel
{
    public class RecentOrderVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }

        public List<RecentOrderItemVM> Items { get; set; }
        = new();
    }
}
