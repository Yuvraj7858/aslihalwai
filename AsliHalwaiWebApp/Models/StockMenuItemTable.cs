using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockMenuItemTable")]
public partial class StockMenuItemTable
{
    [Key]
    [Column("StockMenuItemID")]
    public int StockMenuItemId { get; set; }

    [Column("StockMenuCategoryID")]
    public int StockMenuCategoryId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [ForeignKey("StockItemId")]
    [InverseProperty("StockMenuItemTables")]
    public virtual StockItemTable StockItem { get; set; } = null!;

    [ForeignKey("StockMenuCategoryId")]
    [InverseProperty("StockMenuItemTables")]
    public virtual StockMenuCategoryTable StockMenuCategory { get; set; } = null!;

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("StockMenuItemTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
