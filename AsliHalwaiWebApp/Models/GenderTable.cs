using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Models;

[Table("GenderTable")]
public partial class GenderTable
{
    [Key]
    [Column("GenderID")]
    public int GenderId { get; set; }

    [StringLength(50)]
    public string Gender { get; set; } = null!;

    [InverseProperty("Gender")]
    public virtual ICollection<UserTable> UserTables { get; set; } = new List<UserTable>();
}
