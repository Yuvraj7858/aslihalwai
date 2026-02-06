using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.ViewModel;
using AsliHalwaiWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminOrderController : Controller
{
    private readonly AppDbContext _context;

    public AdminOrderController(AppDbContext context)
    {
        _context = context;
    }

    private bool IsAdmin()
        => HttpContext.Session.GetInt32("UserTypeId") == 1;

    // ======================
    // LIST
    // ======================
    public IActionResult Index()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var orders = _context.OrderTables
            .Join(_context.UserTables,
                  o => o.OrderByUserId,
                  u => u.UserId,
                  (o, u) => new AdminOrderVM
                  {
                      OrderId = o.OrderId,
                      OrderDate = o.OrderDateTime,
                      UserName = u.UserName,
                      Status = o.OrderStatus.OrderStatus
                  })
            .OrderByDescending(x => x.OrderId)
            .ToList();

        return View(orders);
    }

    // ======================
    // APPROVE
    // ======================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int orderId)
    {
        if (HttpContext.Session.GetInt32("UserTypeId") != 1)
            return RedirectToAction("Index", "Home");

        var order = _context.OrderTables
            .Include(o => o.OrderStatus)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return RedirectToAction(nameof(Index));

        // ✅ Sirf Pending → Accept
        if (order.OrderStatus.OrderStatus == "Pending")
        {
            var acceptId = _context.OrderStatusTables
                .Where(s => s.OrderStatus == "Accept")
                .Select(s => s.OrderStatusId)
                .FirstOrDefault();

            if (acceptId == 0)
            {
                TempData["Error"] = "Accept status not found";
                return RedirectToAction(nameof(Index));
            }

            order.OrderStatusId = acceptId;
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }


    // ======================
    // CANCEL
    // ======================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int orderId)
    {
        if (HttpContext.Session.GetInt32("UserTypeId") != 1)
            return RedirectToAction("Index", "Home");

        var order = _context.OrderTables
            .Include(o => o.OrderStatus)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return RedirectToAction(nameof(Index));

        // ❌ Delivered ke baad cancel nahi
        if (order.OrderStatus.OrderStatus != "Delivered")
        {
            var cancelId = _context.OrderStatusTables
                .Where(s => s.OrderStatus == "Cancelled")
                .Select(s => s.OrderStatusId)
                .FirstOrDefault();

            if (cancelId == 0)
            {
                TempData["Error"] = "Cancelled status not found";
                return RedirectToAction(nameof(Index));
            }

            order.OrderStatusId = cancelId;
            order.Description = "Cancelled by Admin";
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkOnTheWay(int orderId)
    {
        if (HttpContext.Session.GetInt32("UserTypeId") != 1)
            return RedirectToAction("Index", "Home");

        var order = _context.OrderTables
            .Include(o => o.OrderStatus)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return RedirectToAction(nameof(Index));

        // ✅ Accept → On the way
        if (order.OrderStatus.OrderStatus == "Accept")
        {
            var onTheWayId = _context.OrderStatusTables
                .Where(s => s.OrderStatus == "On the way")
                .Select(s => s.OrderStatusId)
                .FirstOrDefault();

            if (onTheWayId == 0)
            {
                TempData["Error"] = "On the way status not found";
                return RedirectToAction(nameof(Index));
            }

            order.OrderStatusId = onTheWayId;
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkDelivered(int orderId)
    {
        if (HttpContext.Session.GetInt32("UserTypeId") != 1)
            return RedirectToAction("Index", "Home");

        var order = _context.OrderTables
            .Include(o => o.OrderStatus)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return RedirectToAction(nameof(Index));

        // ✅ On the way → Delivered
        if (order.OrderStatus.OrderStatus == "On the way")
        {
            var deliveredId = _context.OrderStatusTables
                .Where(s => s.OrderStatus == "Delivered")
                .Select(s => s.OrderStatusId)
                .FirstOrDefault();

            if (deliveredId == 0)
            {
                TempData["Error"] = "Delivered status not found";
                return RedirectToAction(nameof(Index));
            }

            order.OrderStatusId = deliveredId;
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult InvoiceDashboard(
     int? orderId,
     string? customerName,
     int? statusId)
    {
        int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
        if (userTypeId != 1)
            return RedirectToAction("Index", "Home");

        var query = _context.OrderTables.AsQueryable();

        // 🔍 FILTERS
        if (orderId.HasValue)
            query = query.Where(o => o.OrderId == orderId.Value);

        if (!string.IsNullOrWhiteSpace(customerName))
            query = query.Where(o =>
                o.OrderReceivedByFullName.Contains(customerName));

        if (statusId.HasValue)
            query = query.Where(o => o.OrderStatusId == statusId.Value);

        var orders =
        (
            from o in query

            join u in _context.UserTables
                on o.OrderByUserId equals u.UserId

            join a in _context.UserAddressTables
                on o.DeliveryAddressUserAddressId equals a.UserAddressId
                into addr

            from ua in addr.DefaultIfEmpty()

            select new AdminInvoiceVM
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDateTime,
                CustomerName = o.OrderReceivedByFullName,
                ContactNo = o.OrderReceivedByContactNo,

                // ✅ ADDRESS
                Address = ua != null ? ua.FullAddress : "",

                OrderStatus = o.OrderStatus.OrderStatus,

                // 🔥 PAYMENT FIELDS (THIS WAS MISSING)
                PaymentMode = o.PaymentMode,
                PaymentStatus = o.PaymentStatus,
                PaymentUTR = o.PaymentUTR,

                TotalAmount = _context.OrderItemDetailTables
                    .Where(i => i.OrderId == o.OrderId)
                    .Sum(i => (decimal)i.UnitPrice * i.Qty)
            }
        )
        .OrderByDescending(x => x.OrderId)
        .ToList();

        var model = new AdminOrderFilterVM
        {
            OrderId = orderId,
            CustomerName = customerName,
            OrderStatusId = statusId,
            Orders = orders
        };

        ViewBag.StatusList = _context.OrderStatusTables.ToList();

        return View(model);
    }

}
