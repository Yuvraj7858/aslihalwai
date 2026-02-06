using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("AddressTypeTable")]
public partial class AddressTypeTable
{
    [Key]
    [Column("AdressTypeID")]
    public int AdressTypeId { get; set; }

    [StringLength(250)]
    public string AddressType { get; set; } = null!;

    [InverseProperty("AddressType")]
    public virtual ICollection<UserAddressTable> UserAddressTables { get; set; } = new List<UserAddressTable>();
}
