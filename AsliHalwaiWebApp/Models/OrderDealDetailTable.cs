using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("OrderDealDetailTable")]
public partial class OrderDealDetailTable
{
    [Key]
    [Column("OrderDealDetailID")]
    public int OrderDealDetailId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("StockDealID")]
    public int StockDealId { get; set; }

    public int Qty { get; set; }

    public double DealPrice { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDealDetailTables")]
    public virtual OrderTable Order { get; set; } = null!;

    [ForeignKey("StockDealId")]
    [InverseProperty("OrderDealDetailTables")]
    public virtual StockDealTable StockDeal { get; set; } = null!;
}
