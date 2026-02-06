using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("UserAddressTable")]
public partial class UserAddressTable
{
    [Key]
    [Column("UserAddressID")]
    public int UserAddressId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("AddressTypeID")]
    public int AddressTypeId { get; set; }

    public string FullAddress { get; set; } = null!;

    [Column("VisibleStatusID")]
    public int VisibleStatusId { get; set; }

    [Column("CreatedBy_UserID")]
    public int CreatedByUserId { get; set; }

    [ForeignKey("AddressTypeId")]
    [InverseProperty("UserAddressTables")]
    public virtual AddressTypeTable AddressType { get; set; } = null!;

    [InverseProperty("DeliveryAddressUserAddress")]
    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    [ForeignKey("UserId")]
    [InverseProperty("UserAddressTables")]
    public virtual UserTable User { get; set; } = null!;

    [ForeignKey("VisibleStatusId")]
    [InverseProperty("UserAddressTables")]
    public virtual VisibleStatusTable VisibleStatus { get; set; } = null!;
}
