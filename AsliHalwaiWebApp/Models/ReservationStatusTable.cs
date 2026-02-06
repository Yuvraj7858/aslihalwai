using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("ReservationStatusTable")]
public partial class ReservationStatusTable
{
    [Key]
    [Column("ReservationStatusID")]
    public int ReservationStatusId { get; set; }

    [StringLength(250)]
    public string ReservationStatus { get; set; } = null!;

    [InverseProperty("ReservationStatud")]
    public virtual ICollection<TableReservationTable> TableReservationTables { get; set; } = new List<TableReservationTable>();
}
