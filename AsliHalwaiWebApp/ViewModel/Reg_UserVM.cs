using System.ComponentModel.DataAnnotations;
namespace AsliHalwaiWebApp.ViewModels
{
    public class Reg_UserVM
    {
        [Required]
        public string UserName { get; set; }   // Email OR Phone

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}