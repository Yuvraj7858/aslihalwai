using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("VisibleStatusTable")]
public partial class VisibleStatusTable
{
    [Key]
    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [StringLength(150)]
    public string VisibleStatus { get; set; } = null!;

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<StockDealDetailTable> StockDealDetailTables { get; set; } = new List<StockDealDetailTable>();

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<StockDealTable> StockDealTables { get; set; } = new List<StockDealTable>();

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<StockItemCategoryTable> StockItemCategoryTables { get; set; } = new List<StockItemCategoryTable>();

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<StockItemTable> StockItemTables { get; set; } = new List<StockItemTable>();

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<StockMenuItemTable> StockMenuItemTables { get; set; } = new List<StockMenuItemTable>();

    [InverseProperty("VisibleStatus")]
    public virtual ICollection<UserAddressTable> UserAddressTables { get; set; } = new List<UserAddressTable>();
}
