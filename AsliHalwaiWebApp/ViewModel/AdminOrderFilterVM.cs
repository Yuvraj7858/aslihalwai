namespace AsliHalwaiWebApp.ViewModel
{
    public class AdminOrderFilterVM
    {
        public int? OrderId { get; set; }
        public string? CustomerName { get; set; }
        public int? OrderStatusId { get; set; }

        public List<AdminInvoiceVM> Orders { get; set; } = new();
    }
}
