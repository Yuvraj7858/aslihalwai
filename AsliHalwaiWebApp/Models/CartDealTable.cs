using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CartDealTable")]
public class CartDealTable
{
    [Key] // 🔥 THIS LINE FIXES EVERYTHING
    public int CartDealId { get; set; }

    public int CartId { get; set; }
    public int StockDealId { get; set; }
    public int Qty { get; set; }
}
