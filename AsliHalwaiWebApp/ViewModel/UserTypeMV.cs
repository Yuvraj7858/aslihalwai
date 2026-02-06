using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModels
{
    public class UserTypeVM
    {
        [Display(Name = "#Unique No")]
        public int UserTypeID { get; set; }

        [Display(Name = "User Type")]
        [Required(ErrorMessage = "Required*")]
        public string UserType { get; set; }

        // List for Index / CRU screen
        public List<UserTypeVM>? UserTypes { get; set; }
    }
}
