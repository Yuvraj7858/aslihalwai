using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModels
{
    public class GenderVM
    {
        [Display(Name = "#Unique No")]
        public int GenderID { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Enter Gender")]
        public string GenderTitle { get; set; }

        // List for UI (Index / CRU page)
        public List<GenderVM>? Genders { get; set; }
    }
}
