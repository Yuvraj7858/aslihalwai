using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("DiscountTable")]
public partial class DiscountTable
{
    [Key]
    [Column("DiscountID")]
    public int DiscountId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DiscountCode { get; set; } = null!;

    public double DiscountPercentage { get; set; }

    public bool IsUseStatus { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<OrderItemDetailTable> OrderItemDetailTables { get; set; } = new List<OrderItemDetailTable>();
}
