using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("UserTypeTable")]
public partial class UserTypeTable
{
    [Key]
    [Column("UserTypeID")]
    public int UserTypeId { get; set; }

    [StringLength(50)]
    public string UserType { get; set; } = null!;

    [InverseProperty("UserType")]
    public virtual ICollection<UserTable> UserTables { get; set; } = new List<UserTable>();
}
