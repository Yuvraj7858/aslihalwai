using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("TableReservationTable")]
public partial class TableReservationTable
{
    [Key]
    [Column("TableReservationID")]
    public int TableReservationId { get; set; }

    [Column("Reservation_UserID")]
    public int? ReservationUserId { get; set; }

    [StringLength(250)]
    public string FullName { get; set; } = null!;

    [StringLength(300)]
    public string EmailAddress { get; set; } = null!;

    [StringLength(14)]
    [Unicode(false)]
    public string MobileNo { get; set; } = null!;

    public DateOnly ReservationRequestDate { get; set; }

    public DateTime ReservationDateTime { get; set; }

    public int NoOfPerson { get; set; }
    [Column("ProccessBy_UserID")]
    public int? ProccessByUserId { get; set; }

    [Column("ReservationStatudID")]
    public int ReservationStatudId { get; set; }

    public string? Description { get; set; }

    [ForeignKey("ProccessByUserId")]
    [InverseProperty("TableReservationTableProccessByUsers")]
    public virtual UserTable ProccessByUser { get; set; } = null!;

    [ForeignKey("ReservationStatudId")]
    [InverseProperty("TableReservationTables")]
    public virtual ReservationStatusTable ReservationStatud { get; set; } = null!;

    [ForeignKey("ReservationUserId")]
    [InverseProperty("TableReservationTableReservationUsers")]
    public virtual UserTable ReservationUser { get; set; } = null!;
}
