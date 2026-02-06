using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("UserTable")]
public partial class UserTable
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("UserTypeID")]
    public int UserTypeId { get; set; }

    [StringLength(150)]
    public string UserName { get; set; } = null!;

    [StringLength(250)]
    public string Password { get; set; } = null!;

    [StringLength(150)]
    public string FirstName { get; set; } = null!;

    [StringLength(150)]
    public string? LastName { get; set; }

    [StringLength(12)]
    public string ContactNo { get; set; } = null!;

    [StringLength(300)]
    public string EmailAddress { get; set; } = null!;

    public DateOnly RegistrationDate { get; set; }

    [Column("UserStatusID")]
    public int UserStatusId { get; set; }

    public DateOnly UserStatusChangeDate { get; set; }

    [Column("GenderID")]
    public int GenderId { get; set; }

    [ForeignKey("GenderId")]
    [InverseProperty("UserTables")]
    public virtual GenderTable Gender { get; set; } = null!;

    [InverseProperty("ProccessByUser")]
    public virtual ICollection<TableReservationTable> TableReservationTableProccessByUsers { get; set; } = new List<TableReservationTable>();

    [InverseProperty("ReservationUser")]
    public virtual ICollection<TableReservationTable> TableReservationTableReservationUsers { get; set; } = new List<TableReservationTable>();

    [InverseProperty("User")]
    public virtual ICollection<UserAddressTable> UserAddressTables { get; set; } = new List<UserAddressTable>();

   
    public virtual  UserDetailTable? UserDetailTable { get; set; }

    [ForeignKey("UserStatusId")]
    [InverseProperty("UserTables")]
    public virtual UserStatusTable UserStatus { get; set; } = null!;

    [ForeignKey("UserTypeId")]
    [InverseProperty("UserTables")]
    public virtual UserTypeTable UserType { get; set; } = null!;
}
