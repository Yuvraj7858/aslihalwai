using AsliHalwaiWebApp.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsliHalwaiWebApp.ViewModels
{
    public class DashboardVM
    {
        public User_ProfileVM ProfileVM { get; set; } = new();

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }

        // User basic info (agar already hai to skip)
        public string UserName { get; set; }

        // Address list
        //public List<UserAddressVM> UserAddresses { get; set; } = new();
        public List<UserAddressListVM> AddressList { get; set; } = new();
        public List<RecentOrderVM> RecentOrders { get; set; }
          = new List<RecentOrderVM>();
    }
}
