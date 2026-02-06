using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Controllers
{
    public class StockController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public StockController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 🔐 ADMIN CHECK
        private bool IsAdmin()
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            return userTypeId == 1;
        }


      
            // ORDER TYPE (ADMIN = 1 , COOK = 3)
            // =====================================================
            [HttpGet]
        public IActionResult OrderType(int id = 0)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            var vm = new OrderTypeVM();

            // Edit mode
            if (id > 0)
            {
                var data = _context.OrderTypeTables.Find(id);
                if (data != null)
                {
                    vm.OrderTypeId = data.OrderTypeId;
                    vm.OrderTypeName = data.OrderTypeName;
                }
            }

            // List
            vm.OrderTypeList = _context.OrderTypeTables
                .Select(x => new OrderTypeVM
                {
                    OrderTypeId = x.OrderTypeId,
                    OrderTypeName = x.OrderTypeName
                })
                .ToList();

            return View(vm);
        }

        // ========================= POST =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderType(OrderTypeVM vm)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                vm.OrderTypeList = _context.OrderTypeTables
                    .Select(x => new OrderTypeVM
                    {
                        OrderTypeId = x.OrderTypeId,
                        OrderTypeName = x.OrderTypeName
                    })
                    .ToList();

                return View(vm);
            }

            if (vm.OrderTypeId == 0)
            {
                // ADD
                _context.OrderTypeTables.Add(new OrderTypeTable
                {
                    OrderTypeName = vm.OrderTypeName
                });
            }
            else
            {
                // UPDATE
                var data = _context.OrderTypeTables.Find(vm.OrderTypeId);
                if (data != null)
                {
                    data.OrderTypeName = vm.OrderTypeName;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("OrderType");
        }

        // ========================= DELETE =========================
        [HttpPost]
        public IActionResult DeleteOrderType(int id)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            var data = _context.OrderTypeTables.Find(id);
            if (data != null)
            {
                _context.OrderTypeTables.Remove(data);
                _context.SaveChanges();
            }

            return RedirectToAction("OrderType");
        }


        // =====================================================
        // STOCK ITEM CATEGORY
        // =====================================================
        [HttpGet]
        public IActionResult StockItemCategory(int id = 0)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            var vm = new StockItemCategoryVM();

            // ===== Visible Status Dropdown
            vm.VisibleStatusList = _context.VisibleStatusTables
                .Select(v => new SelectListItem
                {
                    Value = v.VisibleStatusId.ToString(),
                    Text = v.VisibleStatus
                }).ToList();

            // ===== Edit Mode
            if (id > 0)
            {
                var data = _context.StockItemCategoryTables.Find(id);
                if (data != null)
                {
                    vm.StockItemCategoryId = data.StockItemCategoryId;
                    vm.StockItemCategory = data.StockItemCategory;
                    vm.VisibleStatusId = data.VisibleStatusId;
                }
            }

            // ===== List
            vm.CategoryList = _context.StockItemCategoryTables
                .Select(x => new StockItemCategoryVM
                {
                    StockItemCategoryId = x.StockItemCategoryId,
                    StockItemCategory = x.StockItemCategory,
                    VisibleStatusId = x.VisibleStatusId
                })
                .ToList();

            return View(vm);
        }

        // ===================== POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockItemCategory(StockItemCategoryVM vm)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                vm.VisibleStatusList = _context.VisibleStatusTables
                    .Select(v => new SelectListItem
                    {
                        Value = v.VisibleStatusId.ToString(),
                        Text = v.VisibleStatus
                    }).ToList();

                vm.CategoryList = _context.StockItemCategoryTables
                    .Select(x => new StockItemCategoryVM
                    {
                        StockItemCategoryId = x.StockItemCategoryId,
                        StockItemCategory = x.StockItemCategory,
                        VisibleStatusId = x.VisibleStatusId
                    }).ToList();

                return View(vm);
            }

            if (vm.StockItemCategoryId == 0)
            {
                // ADD
                _context.StockItemCategoryTables.Add(new StockItemCategoryTable
                {
                    StockItemCategory = vm.StockItemCategory,
                    VisibleStatusId = vm.VisibleStatusId,
                    CreatedByUserId = userId.Value
                });
            }
            else
            {
                // UPDATE
                var data = _context.StockItemCategoryTables
                    .Find(vm.StockItemCategoryId);

                if (data != null)
                {
                    data.StockItemCategory = vm.StockItemCategory;
                    data.VisibleStatusId = vm.VisibleStatusId;
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(StockItemCategory));
        }

        // ===================== DELETE =====================
        [HttpPost]
        public IActionResult DeleteStockItemCategory(int id)
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            if (userTypeId != 1 && userTypeId != 3)
                return RedirectToAction("Index", "Home");

            var data = _context.StockItemCategoryTables.Find(id);
            if (data != null)
            {
                _context.StockItemCategoryTables.Remove(data);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(StockItemCategory));
        }

        

        // =========================
        // LIST + EDIT
        // =========================
        [HttpGet]
        public IActionResult StockMenuCategory(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var vm = new StockMenuCategoryVM();

            // LIST
            vm.CategoryList = _context.StockMenuCategoryTables
                .Select(x => new StockMenuCategoryListVM
                {
                    StockMenuCategoryId = x.StockMenuCategoryId,
                    StockMenuCategory = x.StockMenuCategory,
                    CreatedBy = _context.UserTables
                    .Where(u => u.UserId == x.CreatedByUserId)
                    .Select(u => u.UserName)
                    .FirstOrDefault()
                }).ToList();



            // EDIT
            if (id.HasValue && id > 0)
            {
                var entity = _context.StockMenuCategoryTables
                    .Find(id.Value);

                if (entity != null)
                {
                    vm.StockMenuCategoryId = entity.StockMenuCategoryId;
                    vm.StockMenuCategory = entity.StockMenuCategory;
                }
            }

            return View(vm);
        }

        // =========================
        // ADD / UPDATE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockMenuCategory(StockMenuCategoryVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
                return Reload(vm);

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // ADD
            if (vm.StockMenuCategoryId == 0)
            {
                bool exists = _context.StockMenuCategoryTables
                    .Any(x => x.StockMenuCategory == vm.StockMenuCategory);

                if (exists)
                {
                    ModelState.AddModelError("StockMenuCategory", "Already exists");
                    return Reload(vm);
                }

                _context.StockMenuCategoryTables.Add(new StockMenuCategoryTable
                {
                    StockMenuCategory = vm.StockMenuCategory,
                    CreatedByUserId = userId
                });
            }
            // UPDATE
            else
            {
                var entity = _context.StockMenuCategoryTables
                    .Find(vm.StockMenuCategoryId);

                if (entity != null)
                {
                    entity.StockMenuCategory = vm.StockMenuCategory;
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(StockMenuCategory));
        }

        // =========================
        // DELETE
        // =========================
        [HttpPost]
        public IActionResult DeleteStockMenuCategory(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var entity = _context.StockMenuCategoryTables.Find(id);
            if (entity != null)
            {
                _context.StockMenuCategoryTables.Remove(entity);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(StockMenuCategory));
        }

        // 🔁 Reload helper
        private IActionResult Reload(StockMenuCategoryVM vm)
        {
            vm.CategoryList = _context.StockMenuCategoryTables
                .Select(x => new StockMenuCategoryListVM
                {
                    StockMenuCategoryId = x.StockMenuCategoryId,
                    StockMenuCategory = x.StockMenuCategory
                }).ToList();

            return View(vm);
        }

        // =========================
        // GET : STOCK ITEM
        // =========================
        public IActionResult StockItem(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var vm = new StockItemVM();

            LoadDropdowns(vm);

            // EDIT MODE
            if (id.HasValue)
            {
                var item = _context.StockItemTables.Find(id.Value);
                if (item != null)
                {
                    vm.StockItemId = item.StockItemId;
                    vm.StockItemTitle = item.StockItemTitle;
                    vm.ItemSize = item.ItemSize;
                    vm.UnitPrice = item.UnitPrice;
                    vm.StockItemCategoryId = item.StockItemCategoryId;
                    vm.OrderTypeId = item.OrderTypeId;
                    vm.VisibleStatusId = item.VisibleStatusId;
                    vm.ItemPhotoPath = item.ItemPhotoPath;
                }
            }

            // LIST
            vm.ItemList = _context.StockItemTables
                .Select(x => new StockItemListVM
                {
                    StockItemId = x.StockItemId,
                    StockItemTitle = x.StockItemTitle,
                    StockItemCategory = x.StockItemCategory.StockItemCategory,
                    OrderType = x.OrderTypeName.OrderTypeName,
                    ItemSize = x.ItemSize,
                    UnitPrice = x.UnitPrice,
                    VisibleStatus = x.VisibleStatus.VisibleStatus,
                    CreatedBy = x.CreatedByUserId.ToString(),
                    ItemPhotoPath = x.ItemPhotoPath
                }).ToList();

            return View(vm);
        }

        // =========================
        // POST : STOCK ITEM
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockItem(StockItemVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            LoadDropdowns(vm);

            // 🔥 DEBUG: ModelState errors dekhne ke liye
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // 🔥 SAFETY CHECKS
            if (vm.StockItemCategoryId == 0 ||
                vm.OrderTypeId == 0 ||
                vm.VisibleStatusId == 0)
            {
                ModelState.AddModelError("", "Please select all dropdown values.");
                return View(vm);
            }

            StockItemTable item;

            if (vm.StockItemId == 0)
            {
                item = new StockItemTable
                {
                    StockItemTitle = vm.StockItemTitle,
                    ItemSize = vm.ItemSize,
                    UnitPrice = vm.UnitPrice,
                    StockItemCategoryId = vm.StockItemCategoryId,
                    OrderTypeId = vm.OrderTypeId,
                    VisibleStatusId = vm.VisibleStatusId,
                    CreatedByUserId = userId,
                    RegisterDate = DateOnly.FromDateTime(DateTime.Now),

                    // 🔥 DEFAULT IMAGE (IMPORTANT)
                    ItemPhotoPath = "/images/no-image.png"
                };

                _context.StockItemTables.Add(item);
            }
            else
            {
                item = _context.StockItemTables.Find(vm.StockItemId);
                if (item == null) return RedirectToAction("StockItem");

                item.StockItemTitle = vm.StockItemTitle;
                item.ItemSize = vm.ItemSize;
                item.UnitPrice = vm.UnitPrice;
                item.StockItemCategoryId = vm.StockItemCategoryId;
                item.OrderTypeId = vm.OrderTypeId;
                item.VisibleStatusId = vm.VisibleStatusId;
            }

            // 🔥 IMAGE UPLOAD (OPTIONAL)
            if (vm.ItemPhoto != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads/stock");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(vm.ItemPhoto.FileName);
                string path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                vm.ItemPhoto.CopyTo(stream);

                item.ItemPhotoPath = "/uploads/stock/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("StockItem");
        }

        // =========================
        // DELETE
        // =========================
        [HttpPost]
        public IActionResult DeleteStockItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var item = _context.StockItemTables.Find(id);
            if (item != null)
            {
                _context.StockItemTables.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("StockItem");
        }

        // =========================
        private void LoadDropdowns(StockItemVM vm)
        {
            vm.StockItemCategoryList = _context.StockItemCategoryTables
                .Select(x => new SelectListItem
                {
                    Value = x.StockItemCategoryId.ToString(),
                    Text = x.StockItemCategory
                }).ToList();

            vm.OrderTypeList = _context.OrderTypeTables
                .Select(x => new SelectListItem
                {
                    Value = x.OrderTypeId.ToString(),
                    Text = x.OrderTypeName
                }).ToList();

            vm.VisibleStatusList = _context.VisibleStatusTables
                .Select(x => new SelectListItem
                {
                    Value = x.VisibleStatusId.ToString(),
                    Text = x.VisibleStatus
                }).ToList();
        }

        // GET : STOCK ITEM INGREDIENT
        // =========================
        public IActionResult StockItemIngredient(int stockItemId, int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (stockItemId <= 0)
                return RedirectToAction("StockItem");

            var vm = new StockItemIngredientVM
            {
                StockItemId = stockItemId
            };

            if (id.HasValue)
            {
                var ingredient = _context.StockItemIngredientTables
                    .FirstOrDefault(x => x.StockItemIngredientId == id.Value);

                if (ingredient != null)
                {
                    vm.StockItemIngredientId = ingredient.StockItemIngredientId;
                    vm.StockItemIngredientTitle = ingredient.StockItemIngredientTitle;
                    vm.IngredientPhotoPath = ingredient.StockItemIngredientPhoto;
                }
            }

            vm.IngredientList = _context.StockItemIngredientTables
                .Where(x => x.StockItemId == stockItemId)
                .Select(x => new StockItemIngredientListVM
                {
                    StockItemIngredientId = x.StockItemIngredientId,
                    StockItemIngredientTitle = x.StockItemIngredientTitle,
                    IngredientPhotoPath = x.StockItemIngredientPhoto,
                    StockItemTitle = x.StockItem.StockItemTitle,
                    CreatedBy = x.CreatedByUserId.ToString()
                }).ToList();

            return View(vm);
        }


        // =========================
        // POST : STOCK ITEM INGREDIENT
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockItemIngredient(int stockItemId, StockItemIngredientVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (stockItemId <= 0)
                return RedirectToAction("StockItem");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var ingredient = new StockItemIngredientTable
            {
                StockItemId = stockItemId,
                StockItemIngredientTitle = vm.StockItemIngredientTitle,
                CreatedByUserId = userId,
                StockItemIngredientPhoto = null // DB allows NULL now
            };

            _context.StockItemIngredientTables.Add(ingredient);
            _context.SaveChanges(); // 🔥 NOW IT WILL SAVE

            // PHOTO UPLOAD (OPTIONAL)
            if (vm.IngredientPhoto != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads/ingredients");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = ingredient.StockItemIngredientId + ".jpg";
                string path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                vm.IngredientPhoto.CopyTo(stream);

                ingredient.StockItemIngredientPhoto = "/uploads/ingredients/" + fileName;
                _context.SaveChanges();
            }

            return RedirectToAction("StockItemIngredient",
                new { stockItemId = stockItemId });
        }



        [HttpPost]
        public IActionResult DeleteStockItemIngredient(int id, int stockItemId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var ingredient = _context.StockItemIngredientTables.Find(id);
            if (ingredient != null)
            {
                _context.StockItemIngredientTables.Remove(ingredient);
                _context.SaveChanges();
            }

            return RedirectToAction("StockItemIngredient", new { stockItemId });
        }

        //Stock Menu 
        public IActionResult StockMenu()
        {
            var vm = new StockMenuVM();

            // sirf visible categories
            var categories = _context.StockItemCategoryTables
                .Where(c => c.VisibleStatusId == 1)
                .ToList();

            foreach (var cat in categories)
            {
                var catVm = new StockCategoryVM
                {
                    StockItemCategoryId = cat.StockItemCategoryId,
                    StockCategory = cat.StockItemCategory
                };

                // category wise items
                catVm.Items = _context.StockItemTables
                    .Where(i =>
                        i.StockItemCategoryId == cat.StockItemCategoryId &&
                        i.VisibleStatusId == 1)
                    .Select(i => new StockItemListVM
                    {
                        StockItemId = i.StockItemId,
                        StockItemTitle = i.StockItemTitle,
                        StockItemCategory = cat.StockItemCategory,
                        ItemSize = i.ItemSize,
                        UnitPrice = i.UnitPrice,
                        ItemPhotoPath = i.ItemPhotoPath,
                        VisibleStatus = i.VisibleStatus.VisibleStatus,
                        OrderType = i.OrderTypeName.OrderTypeName,
                        CreatedBy = i.CreatedByUserId.ToString()
                    })
                    .ToList();

                vm.Categories.Add(catVm);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult StockDeal(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var vm = new StockDealVM();

            if (id.HasValue)
            {
                var deal = _context.StockDealTables.Find(id.Value);
                if (deal != null)
                {
                    vm.StockDealId = deal.StockDealId;
                    vm.StockDealTitle = deal.StockDealTitle;
                    vm.DealPrice = deal.DealPrice;
                    vm.Discount = deal.Discount;
                    vm.VisibleStatusId = deal.VisibleStatusId;
                    vm.StockDealStartDate = deal.StockDealStartDate;
                    vm.StockDealEndDate = deal.StockDealEndDate;
                    vm.DealPhotoPath = deal.DealPhoto;
                }
            }

            vm.Deals = _context.StockDealTables
                .Select(d => new StockDealListVM
                {
                    StockDealId = d.StockDealId,
                    StockDealTitle = d.StockDealTitle,
                    DealPrice = d.DealPrice,
                    Discount = d.Discount,
                    VisibleStatus = d.VisibleStatus.VisibleStatus,
                    DealPhoto = d.DealPhoto,
                    DealItems = string.Join(" | ",
                        _context.StockDealDetailTables
                            .Where(x => x.StockDealId == d.StockDealId)
                            .Select(x => x.StockItem.StockItemTitle + " (Qty: " + x.Quantity + ")"))
                }).ToList();

            ViewBag.VisibleStatusList = new SelectList(
                _context.VisibleStatusTables, "VisibleStatusId", "VisibleStatus");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockDeal(StockDealVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
                return View(vm);

            StockDealTable deal;

            if (vm.StockDealId == 0)
            {
                deal = new StockDealTable
                {
                    StockDealTitle = vm.StockDealTitle,
                    DealPrice = vm.DealPrice,
                    Discount = vm.Discount,
                    VisibleStatusId = vm.VisibleStatusId,
                    StockDealStartDate = vm.StockDealStartDate,
                    StockDealEndDate = vm.StockDealEndDate,
                    StockDealRegisterDate = DateOnly.FromDateTime(DateTime.Now),
                    DealPhoto = "/images/no-image.png"
                };
                _context.StockDealTables.Add(deal);
                _context.SaveChanges();
            }
            else
            {
                deal = _context.StockDealTables.Find(vm.StockDealId);
                deal.StockDealTitle = vm.StockDealTitle;
                deal.DealPrice = vm.DealPrice;
                deal.Discount = vm.Discount;
                deal.VisibleStatusId = vm.VisibleStatusId;
                deal.StockDealStartDate = vm.StockDealStartDate;
                deal.StockDealEndDate = vm.StockDealEndDate;
            }

            if (vm.DealPhoto != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads/deals");
                Directory.CreateDirectory(folder);

                string fileName = deal.StockDealId + ".jpg";
                string path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                vm.DealPhoto.CopyTo(stream);

                deal.DealPhoto = "/uploads/deals/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("StockDeal");
        }

        [HttpGet]
        public IActionResult StockDealItem(int dealId, int? stockDealDetailId)
        {
            var vm = new StockDealItemVM();
            vm.StockDealId = dealId;

            // Dropdowns
            vm.StockItemList = _context.StockItemTables
                .Select(x => new SelectListItem
                {
                    Value = x.StockItemId.ToString(),
                    Text = x.StockItemTitle
                }).ToList();

            vm.VisibleStatusList = _context.VisibleStatusTables
                .Select(x => new SelectListItem
                {
                    Value = x.VisibleStatusId.ToString(),
                    Text = x.VisibleStatus
                }).ToList();

            // Edit case
            if (stockDealDetailId.HasValue)
            {
                var edit = _context.StockDealDetailTables
                    .FirstOrDefault(x => x.StockDealDetailId == stockDealDetailId);

                if (edit != null)
                {
                    vm.StockDealDetailId = edit.StockDealDetailId;
                    vm.StockItemId = edit.StockItemId;
                    vm.Quantity = edit.Quantity;
                    vm.Discount = edit.Discount;
                    vm.VisibleStatusId = edit.VisibleStatusId;
                }
            }

            // List
            vm.DealItems = _context.StockDealDetailTables
                .Where(x => x.StockDealId == dealId)
                .Select(x => new StockDealItemListVM
                {
                    StockDealDetailId = x.StockDealDetailId,
                    StockDealId = x.StockDealId,
                    StockItemTitle = x.StockItem.StockItemTitle,
                    ItemPhotoPath = x.StockItem.ItemPhotoPath,
                    ItemSize = x.StockItem.ItemSize,
                    OrderType = x.StockItem.OrderTypeName.OrderTypeName,
                    Quantity = x.Quantity,
                    Discount = x.Discount,
                    VisibleStatus = x.VisibleStatus.VisibleStatus
                }).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockDealItem(StockDealItemVM vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(StockDealItem), new { dealId = vm.StockDealId });

            if (vm.StockDealDetailId == 0)
            {
                var item = new StockDealDetailTable
                {
                    StockDealId = vm.StockDealId,
                    StockItemId = vm.StockItemId,
                    Quantity = vm.Quantity,
                    Discount = vm.Discount,
                    VisibleStatusId = vm.VisibleStatusId
                };
                _context.StockDealDetailTables.Add(item);
            }
            else
            {
                var edit = _context.StockDealDetailTables
                    .First(x => x.StockDealDetailId == vm.StockDealDetailId);

                edit.StockItemId = vm.StockItemId;
                edit.Quantity = vm.Quantity;
                edit.Discount = vm.Discount;
                edit.VisibleStatusId = vm.VisibleStatusId;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(StockDealItem),
                new { dealId = vm.StockDealId });
        }


    }
}



  

