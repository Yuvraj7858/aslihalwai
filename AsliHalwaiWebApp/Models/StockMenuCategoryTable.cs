using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("StockMenuCategoryTable")]
public partial class StockMenuCategoryTable
{
    [Key]
    [Column("StockMenuCategoryID")]
    public int StockMenuCategoryId { get; set; }

    [StringLength(300)]
    public string StockMenuCategory { get; set; } = null!;

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [InverseProperty("StockMenuCategory")]
    public virtual ICollection<StockMenuItemTable> StockMenuItemTables { get; set; } = new List<StockMenuItemTable>();
}
