using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class ForgotPasswordVM
    {
        public int UserID { get; set; }

        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password not matched")]
        public string ConfirmPassword { get; set; }
    }

}
