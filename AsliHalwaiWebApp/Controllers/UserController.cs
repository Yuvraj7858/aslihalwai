using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Helpers;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using AsliHalwaiWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace AsliHalwaiWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================
        // LOGIN : GET
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.UserTables.FirstOrDefault(u =>
                u.UserName.Trim().ToLower() == model.UserName.Trim().ToLower() &&
                u.Password.Trim() == model.Password.Trim());

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var status = _context.UserStatusTables
                .FirstOrDefault(s => s.UserStatusId == user.UserStatusId);

            if (status == null || status.UserStatus.ToLower() != "active")
            {
                ModelState.AddModelError("", $"Account is {status?.UserStatus}");
                return View(model);
            }

            // login success ke baad
            var cart = _context.CartTables.FirstOrDefault(x => x.UserId == user.UserId);

            int cartCount = 0;
            if (cart != null)
            {
                cartCount = _context.CartItemDetailTables
                    .Where(x => x.CartId == cart.CartId)
                    .Sum(x => x.Qty);
            }

            HttpContext.Session.SetInt32("CartCount", cartCount);

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetInt32("UserTypeId", user.UserTypeId);


            return RedirectToAction("Index", "Home");
        }


        // =========================
        // LOGOUT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // =========================
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.GenderID = new SelectList(
     _context.GenderTables.ToList(),
     "GenderId",
     "Gender"
 );

            return View(new Reg_UserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Reg_UserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool exists = _context.UserTables.Any(x =>
                x.UserName == model.UserName);

            if (exists)
            {
                ModelState.AddModelError("UserName", "Already registered");
                return View(model);
            }

            var user = new UserTable
            {
                UserTypeId = 3, // customer
                UserName = model.UserName,
                Password = model.Password, // 🔥 later hash
                FirstName = model.UserName, // temp
                ContactNo = model.UserName,
                GenderId = 1,// phone/email
                EmailAddress = model.UserName.Contains("@")
                    ? model.UserName
                    : "",
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                UserStatusId =2 // Active
            };

            _context.UserTables.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Dashboard()
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userTypeId == null || userId == null)
                return RedirectToAction("Index", "Home");

            var dashboard = new DashboardVM();

            // 🔥 RECENT ORDERS
            var recentOrders = _context.OrderTables
       .Where(o => o.OrderByUserId == userId.Value)
       .OrderByDescending(o => o.OrderId)
       .Take(5)
       .Select(o => new RecentOrderVM
       {
           OrderId = o.OrderId,
           OrderDate = o.OrderDateTime,
           Status = o.OrderStatus != null
               ? o.OrderStatus.OrderStatus
               : "N/A",

           Items = o.OrderItemDetailTables
               .Select(i => new RecentOrderItemVM
               {
                   ProductId = i.StockItemId,
                   ProductName = i.StockItem.StockItemTitle
               })
               .ToList()
       })
       .ToList();

            dashboard.RecentOrders = recentOrders;

            // =======================
            // USER PROFILE
            // =======================
            dashboard.ProfileVM = _context.UserTables
                .Where(u => u.UserId == userId.Value)
                .Select(u => new User_ProfileVM
                {
                    UserID = u.UserId,
                    UserTypeId = u.UserTypeId,
                    UserType = u.UserType.UserType,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FirstName + " " + u.LastName,
                    ContactNo = u.ContactNo,
                    GenderTitle = u.Gender.Gender,
                    EmailAddress = u.EmailAddress,
                    RegistrationDate = u.RegistrationDate.ToDateTime(TimeOnly.MinValue),
                    UserStatus = u.UserStatus.UserStatus,
                    UserStatusID = u.UserStatusId,
                    PhotoPath = u.UserDetailTable != null
                        ? u.UserDetailTable.PhotoPath
                        : "/uploads/profile/default.png",
                    AadharCard = u.UserDetailTable != null
                        ? u.UserDetailTable.AadharCard
                        : "",
                    FullAddress = u.UserAddressTables
                        .Select(x => x.FullAddress)
                        .FirstOrDefault(),
                    EducationLevel = u.UserDetailTable != null
                        ? u.UserDetailTable.EducationLevel
                        : "",
                    EducationLastDegreePhotoPath = u.UserDetailTable != null
                        ? u.UserDetailTable.EducationLastDegreeScanPhoto
                        : null,
                    ExperenceLevel = u.UserDetailTable != null
                        ? u.UserDetailTable.ExperenceLevel
                        : "",
                    ExperenceLastPhotoPath = u.UserDetailTable != null
                        ? u.UserDetailTable.LastExperienceScanPhoto
                        : null
                })
                .FirstOrDefault();

            // =======================
            // USER ADDRESSES
            // =======================
            dashboard.AddressList = _context.UserAddressTables
                .Where(a => a.UserId == userId.Value && a.VisibleStatusId == 1)
                .Select(a => new UserAddressListVM
                {
                    UserAddressID = a.UserAddressId,
                    AddressType = a.AddressType.AddressType,
                    FullAddress = a.FullAddress,
                    VisibleStatus = a.VisibleStatus.VisibleStatus,
                    UserName = a.User.UserName
                })
                .ToList();

            return View(dashboard);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Dashboard(DashboardVM dashboardVM)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var user = _context.UserTables.FirstOrDefault(x => x.UserId == userId.Value);
            if (user == null)
                return RedirectToAction("Login", "User");
            // =====================
            // BASIC USER UPDATE
            // =====================
            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.FirstName))
                user.FirstName = dashboardVM.ProfileVM.FirstName;

            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.LastName))
                user.LastName = dashboardVM.ProfileVM.LastName;

            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.EmailAddress))
                user.EmailAddress = dashboardVM.ProfileVM.EmailAddress;

            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.ContactNo))
                user.ContactNo = dashboardVM.ProfileVM.ContactNo;

            // =====================
            // USER DETAIL (ENSURE)
            // =====================
            var userDetail = _context.UserDetailTables.FirstOrDefault(x => x.UserId == userId.Value);

            if (userDetail == null)
            {
                userDetail = new UserDetailTable
                {
                    UserDetailId = userId.Value,
                    UserId = userId.Value,
                    CreatedBy_UserID = userId.Value,
                    UserDetailProvideDate = DateOnly.FromDateTime(DateTime.Now),
                    AadharCard = "NA",
                    EducationLevel = "NA"
                };
                _context.UserDetailTables.Add(userDetail);
            }

            // =====================
            // PROFILE PHOTO
            // =====================
            if (dashboardVM.ProfileVM.UserPhoto != null)
            {
                string fileName = $"{userId.Value}.jpg";

                bool uploaded = FileUploadHelper.UploadPhoto(
                    dashboardVM.ProfileVM.UserPhoto,
                    "uploads/profile",
                    fileName,
                    _env
                );

                if (uploaded)
                {
                    userDetail.PhotoPath = "/uploads/profile/" + fileName;
                    userDetail.UserDetailProvideDate = DateOnly.FromDateTime(DateTime.Now);
                }
            }

            // =====================
            // EDUCATION
            // =====================
            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.EducationLevel))
                userDetail.EducationLevel = dashboardVM.ProfileVM.EducationLevel;

            if (dashboardVM.ProfileVM.EducationLastDegreePhoto != null)
            {
                string eduFile = $"education_{userId.Value}.jpg";

                if (FileUploadHelper.UploadPhoto(
                    dashboardVM.ProfileVM.EducationLastDegreePhoto,
                    "uploads/otherfiles",
                    eduFile,
                    _env))
                {
                    userDetail.EducationLastDegreeScanPhoto = "/uploads/otherfiles/" + eduFile;
                }
            }

            // =====================
            // EXPERIENCE
            // =====================
            if (!string.IsNullOrWhiteSpace(dashboardVM.ProfileVM.ExperenceLevel))
                userDetail.ExperenceLevel = dashboardVM.ProfileVM.ExperenceLevel;

            if (dashboardVM.ProfileVM.ExperenceLastPhoto != null)
            {
                string expFile = $"experience_{userId.Value}.jpg";

                if (FileUploadHelper.UploadPhoto(
                    dashboardVM.ProfileVM.ExperenceLastPhoto,
                    "uploads/otherfiles",
                    expFile,
                    _env))
                {
                    userDetail.LastExperienceScanPhoto = "/uploads/otherfiles/" + expFile;
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Profile Updated Successfully";
            return RedirectToAction("Dashboard");
        }


        //Change Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(DashboardVM model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(model.OldPassword) ||
                string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                TempData["Error"] = "All password fields are required";
                return RedirectToAction("Dashboard");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "New Password and Confirm Password do not match";
                return RedirectToAction("Dashboard");
            }

            var user = _context.UserTables.FirstOrDefault(x => x.UserId == userId.Value);
            if (user == null)
                return RedirectToAction("Login", "User");

            if (user.Password != model.OldPassword)
            {
                TempData["Error"] = "Old Password is incorrect";
                return RedirectToAction("Dashboard");
            }

            // ✅ PASSWORD UPDATE
            user.Password = model.NewPassword;
            _context.SaveChanges();

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction("Dashboard");
        }

       

    }

}
