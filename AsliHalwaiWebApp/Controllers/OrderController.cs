using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Cart_AddItem(int itemid, int qty, string itemtype)
        {
            bool result = false;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(result);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 🛒 Get or Create Cart
                    var cart = _context.CartTables
                        .FirstOrDefault(c => c.UserId == userId);

                    if (cart == null)
                    {
                        cart = new CartTable
                        {
                            UserId = userId.Value
                        };
                        _context.CartTables.Add(cart);
                        _context.SaveChanges();
                    }

                    // 🍕 Normal Item
                    if (itemtype == "item")
                    {
                        var cartItem = _context.CartItemDetailTables
                            .FirstOrDefault(i =>
                                i.CartId == cart.CartId &&
                                i.StockItemId == itemid);

                        if (cartItem != null)
                        {
                            cartItem.Qty += qty;
                        }
                        else
                        {
                            _context.CartItemDetailTables.Add(
                                new CartItemDetailTable
                                {
                                    CartId = cart.CartId,
                                    StockItemId = itemid,
                                    Qty = qty
                                });
                        }
                    }
                    // 🎁 Deal
                    else if (itemtype == "deal")
                    {
                        var cartDeal = _context.CartDealTables
                            .FirstOrDefault(d =>
                                d.CartId == cart.CartId &&
                                d.StockDealId == itemid);

                        if (cartDeal != null)
                        {
                            cartDeal.Qty += qty;
                        }
                        else
                        {
                            _context.CartDealTables.Add(
                                new CartDealTable
                                {
                                    CartId = cart.CartId,
                                    StockDealId = itemid,
                                    Qty = qty
                                });
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    result = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return Json(result);
        }

        [HttpGet]
        public IActionResult ViewCart()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var model = Cart(userId.Value);

            model.OrderTypes = _context.OrderTypeTables
                .Select(o => new SelectListItem
                {
                    Value = o.OrderTypeId.ToString(),   // 🔥 ID
                    Text = o.OrderTypeName              // 🔥 TEXT
                })
                .ToList();

            model.Addresses = _context.UserAddressTables
                .Where(a => a.UserId == userId)
                .Select(a => new SelectListItem
                {
                    Value = a.UserAddressId.ToString(),
                    Text = a.FullAddress
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewCart(ShoppingCartVM model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            // =====================
            // 0️⃣ VERIFY CART EXISTS
            // =====================
            var cart = _context.CartTables
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null)
            {
                TempData["Error"] = "Your cart is empty. Please add items to proceed.";
                return RedirectToAction("AllItems", "Home");
            }

            var cartItems = _context.CartItemDetailTables
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            var cartDeals = _context.CartDealTables
                .Where(cd => cd.CartId == cart.CartId)
                .ToList();

            if (!cartItems.Any() && !cartDeals.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items to proceed.";
                return RedirectToAction("AllItems", "Home");
            }

            // =====================
            // 0️⃣ VERIFY DELIVERY ADDRESS
            // =====================
            if (model.UserAddressId <= 0)
            {
                var defaultAddress = _context.UserAddressTables
                    .FirstOrDefault(a => a.UserId == userId.Value && a.VisibleStatusId == 1);

                if (defaultAddress != null)
                {
                    model.UserAddressId = defaultAddress.UserAddressId;
                }
                else
                {
                    TempData["Error"] = "Please add a delivery address to complete your order.";
                    return RedirectToAction("List_UserAddress", "Setting");
                }
            }

            if (model.OrderTypeId <= 0)
            {
                model.OrderTypeId = _context.OrderTypeTables.Select(o => o.OrderTypeId).FirstOrDefault();
                if (model.OrderTypeId <= 0) model.OrderTypeId = 1;
            }

            using var tx = _context.Database.BeginTransaction();

            try
            {
                // =====================
                // 1️⃣ SAVE ORDER HEADER
                // =====================
                var order = new OrderTable
                {
                    OrderByUserId = userId.Value,
                    OrderDateTime = DateTime.Now,
                    OrderTypeId = model.OrderTypeId,
                    DeliveryAddressUserAddressId = model.UserAddressId,
                    OrderStatusId = 1,

                    OrderReceivedByContactNo = model.ContactNo,
                    OrderReceivedByFullName = (model.FirstName + " " + model.LastName).Trim(),

                    PaymentMode = model.PaymentMode,
                    PaymentUTR = model.PaymentMode == "UPI" ? model.PaymentUTR : null,
                    PaymentStatus = model.PaymentMode == "UPI" ? "Pending" : "COD",

                    Description = "Order placed successfully."
                };

                _context.OrderTables.Add(order);
                _context.SaveChanges();

                // =====================
                // 2️⃣ SAVE CART ITEMS → ORDER ITEMS
                // =====================
                foreach (var ci in cartItems)
                {
                    var stock = _context.StockItemTables
                        .FirstOrDefault(s => s.StockItemId == ci.StockItemId);

                    if (stock == null) continue;

                    _context.OrderItemDetailTables.Add(new OrderItemDetailTable
                    {
                        OrderId = order.OrderId,
                        StockItemId = ci.StockItemId,
                        Qty = ci.Qty,
                        UnitPrice = stock.UnitPrice
                    });
                }

                // =====================
                // 3️⃣ SAVE DEAL ITEMS (IF ANY)
                // =====================
                foreach (var cd in cartDeals)
                {
                    var deal = _context.StockDealTables
                        .FirstOrDefault(d => d.StockDealId == cd.StockDealId);

                    if (deal == null) continue;

                    _context.OrderDealDetailTables.Add(new OrderDealDetailTable
                    {
                        OrderId = order.OrderId,
                        StockDealId = cd.StockDealId,
                        Qty = cd.Qty,
                        DealPrice = deal.DealPrice
                    });
                }

                _context.SaveChanges();

                // =====================
                // 4️⃣ CLEAR CART
                // =====================
                _context.CartItemDetailTables.RemoveRange(cartItems);
                _context.CartDealTables.RemoveRange(cartDeals);
                _context.CartTables.Remove(cart);

                _context.SaveChanges();

                tx.Commit();
                return RedirectToAction("OrderSuccess");
            }
            catch
            {
                tx.Rollback();
                TempData["Error"] = "Unable to process your order at this time. Please check your information and try again.";
                return RedirectToAction("ViewCart");
            }
        }


        public IActionResult OrderSuccess()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UpdateQty(int cartItemId, string itemType, int change)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(false);

            if (itemType == "item")
            {
                var item = _context.CartItemDetailTables
                    .FirstOrDefault(x => x.CartItemId == cartItemId);

                if (item == null) return Json(false);

                item.Qty += change;

                if (item.Qty <= 0)
                    _context.CartItemDetailTables.Remove(item);
            }
            else
            {
                var deal = _context.CartDealTables
                    .FirstOrDefault(x => x.CartDealId == cartItemId);

                if (deal == null) return Json(false);

                deal.Qty += change;

                if (deal.Qty <= 0)
                    _context.CartDealTables.Remove(deal);
            }

            _context.SaveChanges();
            return Json(true);
        }


        [HttpPost]
        public IActionResult RemoveItem(int cartItemId, string itemType)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(false);

            if (itemType == "item")
            {
                var item = _context.CartItemDetailTables
                    .FirstOrDefault(x => x.CartItemId == cartItemId);

                if (item != null)
                    _context.CartItemDetailTables.Remove(item);
            }
            else
            {
                var deal = _context.CartDealTables
                    .FirstOrDefault(x => x.CartDealId == cartItemId);

                if (deal != null)
                    _context.CartDealTables.Remove(deal);
            }

            _context.SaveChanges();
            return Json(true);
        }

        private ShoppingCartVM Cart(int userId)
        {
            var user = _context.UserTables.FirstOrDefault(u => u.UserId == userId);

            var shoppingCart = new ShoppingCartVM(
                user?.FirstName ?? "",
                user?.LastName ?? "",
                user?.ContactNo ?? ""
            );

            var cart = _context.CartTables.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
                return shoppingCart;

            int cartId = cart.CartId;

            // 🍕 NORMAL ITEMS
            var cartItems = _context.CartItemDetailTables
                .Where(ci => ci.CartId == cartId)
                .ToList();

            foreach (var ci in cartItems)
            {
                var item = _context.StockItemTables
                    .FirstOrDefault(s => s.StockItemId == ci.StockItemId);

                if (item == null) continue;

                shoppingCart.CartItems.Add(new CartVM
                {
                    CartId = cartId,
                    CartItemId = ci.CartItemId,
                    StockItemId = ci.StockItemId,
                    StockItemTitle = item.StockItemTitle,
                    StockItemCategory = item.StockItemCategory?.StockItemCategory ?? "",
                    ItemPhotoPath = item.ItemPhotoPath,
                    ItemSize = item.ItemSize,
                    UnitPrice = (decimal)item.UnitPrice,
                    Qty = ci.Qty,
                    ItemType = "item"
                });
            }

            // 🎁 DEAL ITEMS
            var cartDeals = _context.CartDealTables
                .Where(cd => cd.CartId == cartId)
                .ToList();

            foreach (var cd in cartDeals)
            {
                var deal = _context.StockDealTables
                    .FirstOrDefault(d => d.StockDealId == cd.StockDealId);

                if (deal == null) continue;

                shoppingCart.CartItems.Add(new CartVM
                {
                    CartId = cartId,
                    CartItemId = cd.CartDealId,
                    StockItemId = cd.StockDealId,
                    StockItemTitle = deal.StockDealTitle,
                    StockItemCategory = "Deal",
                    ItemPhotoPath = deal.DealPhoto,
                    ItemSize = "Normal",
                    UnitPrice = (decimal)deal.DealPrice,
                    Qty = cd.Qty,
                    ItemType = "deal"
                });
            }

            return shoppingCart;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            var order = _context.OrderTables
     .Where(o => o.OrderId == id && o.OrderByUserId == userId.Value)
     .Select(o => new OrderDetailsVM
     {
         OrderId = o.OrderId,
         OrderDate = o.OrderDateTime,
         Status = o.OrderStatus.OrderStatus,

         // ✅ PAYMENT FIELDS (MISSING THE)
         PaymentMode = o.PaymentMode,
         PaymentStatus = o.PaymentStatus,
         PaymentUTR = o.PaymentUTR,

         Items = _context.OrderItemDetailTables
             .Where(i => i.OrderId == o.OrderId)
             .Select(i => new OrderDetailsItemVM
             {
                 ProductName = i.StockItem.StockItemTitle,
                 PhotoPath = i.StockItem.ItemPhotoPath,
                 Qty = i.Qty,
                 UnitPrice = (decimal)i.UnitPrice
             })
             .ToList()
     })
     .FirstOrDefault();


            if (order == null)
                return RedirectToAction("Dashboard", "User");

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(int orderId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var order = _context.OrderTables
                .FirstOrDefault(o =>
                    o.OrderId == orderId &&
                    o.OrderByUserId == userId.Value);

            if (order == null)
                return RedirectToAction("Dashboard", "User");

            // 🔴 Only Pending orders can be cancelled
            if (order.OrderStatusId != 1)
            {
                TempData["Error"] = "Order cannot be cancelled now.";
                return RedirectToAction("Dashboard", "User");
            }

            // ✅ Cancel order
            order.OrderStatusId = 5; // Cancelled
            order.Description = "Order cancelled by customer";

            _context.SaveChanges();

            TempData["Success"] = "Order cancelled successfully.";
            return RedirectToAction("Dashboard", "User");
        }


        [HttpGet]
        public IActionResult Invoice(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var query = _context.OrderTables
                .Where(o => o.OrderId == id);

            // Normal user → only own invoice
            if (userTypeId != 1)
                query = query.Where(o => o.OrderByUserId == userId.Value);

            var invoice = query
                .Select(o => new InvoiceVM
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDateTime,
                    CustomerName = o.OrderReceivedByFullName,
                    ContactNo = o.OrderReceivedByContactNo,
                    Address = o.DeliveryAddressUserAddress != null ? o.DeliveryAddressUserAddress.FullAddress : "Takeaway / Store Pickup",
                    OrderStatus = o.OrderStatus != null ? o.OrderStatus.OrderStatus : "Pending",

                    Items = _context.OrderItemDetailTables
                        .Where(i => i.OrderId == o.OrderId)
                        .Select(i => new InvoiceItemVM
                        {
                            ProductName = i.StockItem.StockItemTitle,
                            Qty = i.Qty,
                            UnitPrice = (decimal)i.UnitPrice
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (invoice == null)
                return RedirectToAction("Dashboard", "User");

            invoice.SubTotal = invoice.Items.Sum(x => x.Total);

            return View(invoice);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(0);

            var cart = _context.CartTables.FirstOrDefault(c => c.UserId == userId.Value);

            int count = 0;

            if (cart != null)
            {
                count =
                    _context.CartItemDetailTables.Count(x => x.CartId == cart.CartId)
                  + _context.CartDealTables.Count(x => x.CartId == cart.CartId);
            }

            HttpContext.Session.SetInt32("CartCount", count);

            return Json(count);
        }

        [HttpGet]
        public IActionResult UpiPayment(int orderId)
        {
            var order = _context.OrderTables.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return RedirectToAction("Index", "Home");

            decimal itemsTotal = _context.OrderItemDetailTables
                .Where(i => i.OrderId == orderId)
                .Sum(i => (decimal?)i.UnitPrice * i.Qty) ?? 0m;

            decimal dealsTotal = _context.OrderDealDetailTables
                .Where(d => d.OrderId == orderId)
                .Sum(d => (decimal?)d.DealPrice * d.Qty) ?? 0m;

            var vm = new UpiPaymentVM
            {
                OrderId = order.OrderId,
                Amount = itemsTotal + dealsTotal
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitUpiPayment(UpiPaymentVM vm)
        {
            var order = _context.OrderTables.FirstOrDefault(o => o.OrderId == vm.OrderId);
            if (order == null) return RedirectToAction("Index", "Home");

            order.PaymentMode = "UPI";
            order.PaymentStatus = "Pending";
            order.PaymentUTR = vm.PaymentUTR;

            _context.SaveChanges();

            TempData["Success"] = "Payment submitted. Verification pending.";
            return RedirectToAction("Details", new { id = vm.OrderId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkPaid(int orderId)
        {
            var order = _context.OrderTables.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return RedirectToAction("InvoiceDashboard", "AdminOrder");

            order.PaymentStatus = "Paid";
            _context.SaveChanges();

            return RedirectToAction("InvoiceDashboard", "AdminOrder");
        }

    }
}
