using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockDealDetailTable")]
public partial class StockDealDetailTable
{
    [Key]
    [Column("StockDealDetailID")]
    public int StockDealDetailId { get; set; }

    [Column("StockDealID")]
    public int StockDealId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    public double Discount { get; set; }

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("StockDealId")]
    [InverseProperty("StockDealDetailTables")]
    public virtual StockDealTable StockDeal { get; set; } = null!;

    [ForeignKey("StockItemId")]
    [InverseProperty("StockDealDetailTables")]
    public virtual StockItemTable StockItem { get; set; } = null!;

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("StockDealDetailTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
