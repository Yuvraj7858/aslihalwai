public class OrderDetailsVM
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "";

    public string? PaymentMode { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentUTR { get; set; }

    public List<OrderDetailsItemVM> Items { get; set; } = new();
}

public class OrderDetailsItemVM
{
    public string ProductName { get; set; } = "";
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public string PhotoPath { get; set; } = "";
}
