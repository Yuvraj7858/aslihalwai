using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsliHalwaiWebApp.Models
{
    [Table("UserPasswordRecoveryTable")]
    public class UserPasswordRecoveryTable
    {
        [Key]
        public int UserPasswordRecoveryID { get; set; }

        public int UserID { get; set; }

        [Required]
        public string RecoveryCode { get; set; }

        public DateTime ExpiryDateTime { get; set; }

        public bool RecoveryStatus { get; set; }

        public string OldPassword { get; set; }

        [ForeignKey(nameof(UserID))]
        public UserTable User { get; set; }
    }

}
