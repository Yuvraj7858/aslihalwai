using System.ComponentModel.DataAnnotations;

namespace  AsliHalwaiWebApp.ViewModel
{ 
public class StockDealVM
{
    public int StockDealId { get; set; }

    [Required]
    public string StockDealTitle { get; set; }

    [Required]
    public double DealPrice { get; set; }

    public double Discount { get; set; }

    [Required]
    public int VisibleStatusId { get; set; }

    public DateOnly StockDealStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly StockDealEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

    public IFormFile? DealPhoto { get; set; }
    public string? DealPhotoPath { get; set; }

    // Deal List
    public List<StockDealListVM> Deals { get; set; } = new();
}

public class StockDealListVM
{
    public int StockDealId { get; set; }
    public string StockDealTitle { get; set; }
    public double DealPrice { get; set; }
    public double Discount { get; set; }
    public string VisibleStatus { get; set; }
    public string DealPhoto { get; set; }
    public string DealItems { get; set; }
}
}