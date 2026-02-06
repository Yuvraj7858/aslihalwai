using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModels
{
    public class VisibleStatusVM
    {
        // ✅ Final rule: Id (not ID)
        public int VisibleStatusID { get; set; }

        [Display(Name = "Visible Status")]
        [Required(ErrorMessage = "Field Required*")]
        public string VisibleStatus { get; set; }

        // List for UI (Index / CRU screen)
        public List<VisibleStatusVM>? VisibleStatuses { get; set; }
    }
}
