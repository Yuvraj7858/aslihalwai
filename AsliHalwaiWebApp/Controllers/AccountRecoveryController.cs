using AsliHalwaiWebApp.CommonCode;
using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AsliHalwaiWebApp.Controllers
{
    public class AccountRecoveryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailSender _email;

        public AccountRecoveryController(AppDbContext context, EmailSender email)
        {
            _context = context;
            _email = email;
        }

        // =========================
        // FORGOT PASSWORD
        // =========================
        [HttpGet]
        public IActionResult AccountRecovery()
        {
            return View(new AccountRecoveryVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountRecovery(AccountRecoveryVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.UserTables
                .FirstOrDefault(x =>
                    x.UserName == model.UserName ||
                    x.EmailAddress == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("UserName", "User not found");
                return View(model);
            }

            string code = Guid.NewGuid().ToString();

            var recovery = new UserPasswordRecoveryTable
            {
                UserID = user.UserId,
                RecoveryCode = code,
                ExpiryDateTime = DateTime.Now.AddHours(24),
                RecoveryStatus = true,
                OldPassword = user.Password
            };

            _context.UserPasswordRecoveryTables.Add(recovery);
            _context.SaveChanges();

            string link = Url.Action(
                "ResetPassword",
                "AccountRecovery",
                new { code },
                Request.Scheme);

            string body = $@"
                Hello {user.UserName},<br/><br/>
                Click below link to reset password:<br/>
                <a href='{link}'>Reset Password</a>";

            _email.SendEmail(user.EmailAddress, "Reset Password", body);

            ViewBag.Message = "Recovery link sent to your email";
            return View(model);
        }

        // =========================
        // RESET PASSWORD
        // =========================
        [HttpGet]
        public IActionResult ResetPassword(string code)
        {
            var recovery = _context.UserPasswordRecoveryTables
                .FirstOrDefault(x =>
                    x.RecoveryCode == code &&
                    x.RecoveryStatus &&
                    x.ExpiryDateTime > DateTime.Now);

            if (recovery == null)
                return RedirectToAction("Login", "User");

            var user = _context.UserTables.Find(recovery.UserID);

            return View(new ForgotPasswordVM
            {
                UserID = user.UserId,
                UserName = user.UserName,
                EmailAddress = user.EmailAddress
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.UserTables.Find(model.UserID);
            if (user == null)
                return RedirectToAction("Login", "User");

            var recoveries = _context.UserPasswordRecoveryTables
                .Where(x => x.UserID == model.UserID && x.RecoveryStatus);

            foreach (var item in recoveries)
                item.RecoveryStatus = false;

            user.Password = model.NewPassword;
            _context.SaveChanges();

            return RedirectToAction("Login", "User");
        }
    }
}
