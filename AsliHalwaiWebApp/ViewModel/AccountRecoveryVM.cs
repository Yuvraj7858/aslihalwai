using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModel
{
    public class AccountRecoveryVM
    {
        [Required(ErrorMessage = "UserName or Email is required")]
        public string UserName { get; set; }
    }
}
