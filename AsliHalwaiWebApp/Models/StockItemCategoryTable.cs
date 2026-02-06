using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockItemCategoryTable")]
public partial class StockItemCategoryTable
{
    [Key]
    [Column("StockItemCategoryID")]
    public int StockItemCategoryId { get; set; }

    [StringLength(500)]
    public string StockItemCategory { get; set; } = null!;

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [InverseProperty("StockItemCategory")]
    public virtual ICollection<StockItemTable> StockItemTables { get; set; } = new List<StockItemTable>();

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("StockItemCategoryTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
