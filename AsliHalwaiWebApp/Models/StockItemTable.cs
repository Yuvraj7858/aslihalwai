using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockItemTable")]
public partial class StockItemTable
{
    [Key]
    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Column("StockItemCategoryID")]
    public int StockItemCategoryId { get; set; }

    [StringLength(500)]
    public string ItemPhotoPath { get; set; } = null!;

    [StringLength(350)]
    public string StockItemTitle { get; set; } = null!;

    [StringLength(150)]
    public string ItemSize { get; set; } = null!;

    public double UnitPrice { get; set; }

    public DateOnly RegisterDate { get; set; }

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [Column("OrderTypeID")]
    public int OrderTypeId { get; set; }

    [InverseProperty("StockItem")]
    public virtual ICollection<OrderItemDetailTable> OrderItemDetailTables { get; set; } = new List<OrderItemDetailTable>();

    [ForeignKey("OrderTypeId")]
    [InverseProperty("StockItemTables")]
    public virtual OrderTypeTable OrderTypeName { get; set; } = null!;

    [InverseProperty("StockItem")]
    public virtual ICollection<StockDealDetailTable> StockDealDetailTables { get; set; } = new List<StockDealDetailTable>();

    [ForeignKey("StockItemCategoryId")]
    [InverseProperty("StockItemTables")]
    public virtual StockItemCategoryTable StockItemCategory { get; set; } = null!;

    [InverseProperty("StockItem")]
    public virtual ICollection<StockItemIngredientTable> StockItemIngredientTables { get; set; } = new List<StockItemIngredientTable>();

    [InverseProperty("StockItem")]
    public virtual ICollection<StockMenuItemTable> StockMenuItemTables { get; set; } = new List<StockMenuItemTable>();

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("StockItemTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
