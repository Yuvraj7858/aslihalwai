using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("OrderStatusTable")]
public partial class OrderStatusTable
{
    [Key]
    [Column("OrderStatusID")]
    public int OrderStatusId { get; set; }

    [StringLength(250)]
    public string OrderStatus { get; set; } = null!;

    [InverseProperty("OrderStatus")]
    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();
}
