using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AsliHalwaiWebApp.ViewModel
{
    public class UserAddressVM
    {
        // ===== Add / Edit =====
        public int UserAddressID { get; set; }
        public int AddressTypeID { get; set; }
        public string FullAddress { get; set; }
        public int VisibleStatusID { get; set; }

        // ===== Dropdowns (🔥 FIX) =====
        public List<SelectListItem> AddressTypes { get; set; } = new();
        public List<SelectListItem> VisibleStatuses { get; set; } = new();

        // ===== List =====
        public List<UserAddressListVM> AddressList { get; set; } = new();
    }

    public class UserAddressListVM
    {
        public int UserAddressID { get; set; }
        public string AddressType { get; set; }
        public string FullAddress { get; set; }
        public string VisibleStatus { get; set; }
        public string UserName { get; set; }
    }
}
