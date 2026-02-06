using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("OrderItemDetailTable")]
public partial class OrderItemDetailTable
{
    [Key]
    [Column("OrderItemDetailID")]
    public int OrderItemDetailId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    public int Qty { get; set; }

    public double UnitPrice { get; set; }

    [Column("DiscountID")]
    public int? DiscountId { get; set; }

    public double DiscountAmount { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("OrderItemDetailTables")]
    public virtual DiscountTable? Discount { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItemDetailTables")]
    public virtual OrderTable Order { get; set; } = null!;

    [ForeignKey("StockItemId")]
    [InverseProperty("OrderItemDetailTables")]
    public virtual StockItemTable StockItem { get; set; } = null!;
}
