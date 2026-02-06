using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("UserStatusTable")]
public partial class UserStatusTable
{
    [Key]
    [Column("UserStatusID")]
    public int UserStatusId { get; set; }

    [StringLength(150)]
    public string UserStatus { get; set; } = null!;

    [InverseProperty("UserStatus")]
    public virtual ICollection<UserTable> UserTables { get; set; } = new List<UserTable>();
}
