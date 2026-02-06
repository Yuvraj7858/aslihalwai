using AsliHalwaiWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CartItemDetailTable")] // 🔥 EXACT DB TABLE NAME
public class CartItemDetailTable
{
    [Key]
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int StockItemId { get; set; }
    public StockItemTable StockItem { get; set; }
    public int Qty { get; set; }
}
