using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockDealTable")]
public partial class StockDealTable
{
    [Key]
    [Column("StockDealID")]
    public int StockDealId { get; set; }

    [StringLength(150)]
    public string StockDealTitle { get; set; } = null!;

    public double DealPrice { get; set; }

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    public DateOnly StockDealStartDate { get; set; }

    public double Discount { get; set; }

    public DateOnly StockDealEndDate { get; set; }

    public DateOnly StockDealRegisterDate { get; set; }

    public string DealPhoto { get; set; } 

    [InverseProperty("StockDeal")]
    public virtual ICollection<OrderDealDetailTable> OrderDealDetailTables { get; set; } = new List<OrderDealDetailTable>();

    [InverseProperty("StockDeal")]
    public virtual ICollection<StockDealDetailTable> StockDealDetailTables { get; set; } = new List<StockDealDetailTable>();

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("StockDealTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
