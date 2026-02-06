using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModels
{
    public class UserStatusVM
    {
        [Display(Name = "#Unique No")]
        public int UserStatusID { get; set; }

        [Display(Name = "Account Status")]
        [Required(ErrorMessage = "Field Required*")]
        public string UserStatus { get; set; }

        // List for UI (Index / CRU screen)
        public List<UserStatusVM>? UserStatuses { get; set; }
    }
}
