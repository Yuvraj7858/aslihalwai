// ========================= 1) OrderTypeTable.cs =========================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsliHalwaiWebApp.Models;

[Table("OrderTypeTable")]
public partial class OrderTypeTable
{
    [Key]
    [Column("OrderTypeID")]
    public int OrderTypeId { get; set; }

    [Column("OrderTypeName")]
    [StringLength(150)]
    public string OrderTypeName { get; set; } = null!;

    // Orders linked by OrderTypeId
    [InverseProperty(nameof(OrderTable.OrderType))]
    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    // Stock items linked by OrderTypeId
    [InverseProperty(nameof(StockItemTable.OrderTypeName))]
    public virtual ICollection<StockItemTable> StockItemTables { get; set; } = new List<StockItemTable>();
}
