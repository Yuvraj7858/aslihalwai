namespace AsliHalwaiWebApp.ViewModel
{
    public class AdminOrderVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        //public string Address { get; set; }

        public string UserName { get; set; } = "";
        public string Status { get; set; } = "";
    }

}
