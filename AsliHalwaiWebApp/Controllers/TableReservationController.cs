using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Controllers
{
    public class TableReservationController : Controller
    {
        private readonly AppDbContext _context;

        public TableReservationController(AppDbContext context)
        {
            _context = context;
        }

        // ================= LIST + PROCESS =================
        [HttpGet]
        public IActionResult BookingTables(int id = 0)
        {
            // ✅ CORRECT SESSION KEYS
            var userId = HttpContext.Session.GetInt32("UserId");
            var userTypeId = HttpContext.Session.GetInt32("UserTypeId");

            if (!userId.HasValue || !userTypeId.HasValue)
                return RedirectToAction("Index", "Home");

            var vm = new TableReservationVM();

            var query = _context.TableReservationTables
                .Include(x => x.ReservationUser)
                .Include(x => x.ProccessByUser)
                .Include(x => x.ReservationStatud)
                .AsQueryable();

            // Regular users / customers → see only their own bookings
            if (userTypeId != 1 && userTypeId != 2)
                query = query.Where(x => x.ReservationUserId == userId);

            var list = query.OrderByDescending(x => x.TableReservationId).ToList();

            foreach (var item in list)
            {
                vm.ReservationList.Add(new TableReservationVM
                {
                    TableReservationId = item.TableReservationId,
                    FullName = item.FullName,
                    MobileNo = item.MobileNo,
                    ReservationDateTime = item.ReservationDateTime,
                    NoOfPerson = item.NoOfPerson,
                    ReservationStatus = item.ReservationStatud != null ? item.ReservationStatud.ReservationStatus : "Pending",
                    Description = item.Description,
                    BookingUserName = item.ReservationUser != null ? item.ReservationUser.UserName : "Guest",
                    ProcessByUser = item.ProccessByUser != null ? item.ProccessByUser.UserName : ""
                });
            }

            // ADMIN PROCESS CLICK
            if (id > 0 && (userTypeId == 1 || userTypeId == 2 || userTypeId == 3))
            {
                var edit = _context.TableReservationTables.FirstOrDefault(x => x.TableReservationId == id);
                if (edit != null)
                {
                    vm.TableReservationId = edit.TableReservationId;
                    vm.FullName = edit.FullName;
                    vm.MobileNo = edit.MobileNo;
                    vm.ReservationDate = edit.ReservationDateTime.Date;
                    vm.ReservationTime = edit.ReservationDateTime.ToString("hh:mm tt");
                    vm.Description = edit.Description;
                    vm.ReservationStatudId = edit.ReservationStatudId;
                }
            }

            ViewBag.ReservationStatusId = new SelectList(
                _context.ReservationStatusTables,
                "ReservationStatusId",
                "ReservationStatus"
            );

            return View(vm);
        }

        // ================= ADMIN PROCESS POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookingTables(TableReservationVM model)
        {
            var adminId = HttpContext.Session.GetInt32("UserId");
            var userTypeId = HttpContext.Session.GetInt32("UserTypeId");

            if (!adminId.HasValue || !(userTypeId == 1 || userTypeId == 2 || userTypeId == 3))
                return RedirectToAction("Index", "Home");

            var reservation = _context.TableReservationTables
                .FirstOrDefault(x => x.TableReservationId == model.TableReservationId);

            if (reservation != null)
            {
                reservation.ReservationStatudId = model.ReservationStatudId;
                reservation.Description = model.Description;
                reservation.ProccessByUserId = adminId;
                _context.SaveChanges();
            }

            return RedirectToAction("BookingTables");
        }

        // ================= USER CANCEL =================
        public IActionResult CancelBooking(int bookingtableid)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userTypeId = HttpContext.Session.GetInt32("UserTypeId");

            if (!userId.HasValue)
                return RedirectToAction("Index", "Home");

            var reservation = _context.TableReservationTables
                .FirstOrDefault(x => x.TableReservationId == bookingtableid);

            if (reservation != null && (userTypeId == 1 || reservation.ReservationUserId == userId.Value))
            {
                if (reservation.ReservationStatudId == 1)
                {
                    reservation.ReservationStatudId = 4;
                    reservation.Description = "Canceled By User";
                    reservation.ProccessByUserId = userId;
                    _context.SaveChanges();
                    TempData["ReservationSuccess"] = "Reservation cancelled successfully.";
                }
            }

            return RedirectToAction("BookingTables");
        }

        // ================= CREATE BOOKING =================
        [HttpPost]
        public IActionResult BookTable(TableReservationVM model)
        {
            if (!ModelState.IsValid || model.ReservationDate == null)
            {
                TempData["ReservationError"] = "Invalid reservation details. Please check all fields.";
                return RedirectToAction("Index", "Home");
            }

            int? userId = HttpContext.Session.GetInt32("UserId"); // Safe session key

            TimeSpan timeOfDay;
            if (!string.IsNullOrWhiteSpace(model.ReservationTime) && DateTime.TryParse(model.ReservationTime, out var parsedTime))
            {
                timeOfDay = parsedTime.TimeOfDay;
            }
            else
            {
                timeOfDay = new TimeSpan(19, 0, 0); // Default 7:00 PM
            }

            var dateTime = model.ReservationDate.Value.Date.Add(timeOfDay);

            var reservation = new TableReservationTable
            {
                ReservationUserId = userId, // null = guest
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                MobileNo = model.MobileNo,
                ReservationRequestDate = DateOnly.FromDateTime(DateTime.Now),
                ReservationDateTime = dateTime,
                NoOfPerson = model.NoOfPerson ?? 2,
                ReservationStatudId = 1, // Pending
                Description = userId == null ? "Guest Booking" : "User Booking"
            };

            _context.TableReservationTables.Add(reservation);
            _context.SaveChanges();

            TempData["ReservationSuccess"] = "Table booked successfully!";
            return RedirectToAction("Index", "Home");
        }

    }
}
