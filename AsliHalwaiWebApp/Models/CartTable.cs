using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CartTable")] // 🔥 EXACT DB TABLE NAME
public class CartTable
{
    [Key]
    public int CartId { get; set; }

    public int UserId { get; set; }
}
