using AsliHalwaiWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("UserDetailTable")]
public partial class UserDetailTable
{
    // 🔑 PK (also FK)
    [Key]
    public int UserDetailId { get; set; }

    // 🔗 FK
    public int UserId { get; set; }

    // ✅ DATE (THIS MUST STAY)
    public DateOnly UserDetailProvideDate { get; set; }

    public string? PhotoPath { get; set; }
    public string? AadharCard { get; set; }
    public string? EducationLevel { get; set; }

    public string? ExperenceLevel { get; set; }
    public string? EducationLastDegreeScanPhoto { get; set; }
    public string? LastExperienceScanPhoto { get; set; }

    public int CreatedBy_UserID { get; set; }

    // ✅ THIS WAS MISSING (ROOT CAUSE)
    public virtual UserTable User { get; set; } = null!;
}
