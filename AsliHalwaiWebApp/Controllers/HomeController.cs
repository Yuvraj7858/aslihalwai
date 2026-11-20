using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using AsliHalwaiWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AsliHalwaiWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new HomeVM();

            // =========================
            // MENU = STOCK ITEM CATEGORY
            // =========================
            var categories = _context.StockItemCategoryTables
                .Where(c => c.VisibleStatusId == 1)
                .ToList();

            foreach (var cat in categories)
            {
                var categoryVM = new MenuCategoryVM
                {
                    StockMenuCategoryId = cat.StockItemCategoryId,
                    MenuCategory = cat.StockItemCategory
                };

                // ITEMS FROM StockItemTable
                categoryVM.Lists = _context.StockItemTables
                    .Where(i =>
                        i.StockItemCategoryId == cat.StockItemCategoryId &&
                        i.VisibleStatusId == 1)
                    .Select(i => new HomeStockItemVM
                    {
                        StockItemId = i.StockItemId,
                        StockItemTitle = i.StockItemTitle,
                        ItemSize = i.ItemSize,
                        UnitPrice = i.UnitPrice,
                        ItemPhotoPath = string.IsNullOrEmpty(i.ItemPhotoPath)
                                            ? "/images/no-image.png"
                                            : i.ItemPhotoPath,
                        OrderType = i.OrderTypeName.OrderTypeName,
                        VisibleStatus = i.VisibleStatus.VisibleStatus
                    })
                    .ToList();

                vm.Menu.Add(categoryVM);
            }

            // =========================
            // POPULAR DISHES (OPTIONAL)
            // =========================
            vm.PopularDishes = _context.StockItemTables
                .Where(i => i.VisibleStatusId == 1)
                .OrderByDescending(i => i.StockItemId)
                .Take(10)
                .Select(i => new HomeStockItemVM
                {
                    StockItemId = i.StockItemId,
                    StockItemTitle = i.StockItemTitle,
                    ItemSize = i.ItemSize,
                    UnitPrice = i.UnitPrice,
                    ItemPhotoPath = string.IsNullOrEmpty(i.ItemPhotoPath)
                                        ? "/images/no-image.png"
                                        : i.ItemPhotoPath,
                    OrderType = i.OrderTypeName.OrderTypeName,
                    VisibleStatus = i.VisibleStatus.VisibleStatus
                })
                .ToList();

            return View(vm);
        }


        [HttpGet]
        public IActionResult AllItems(string? searchKey, int? categoryId)
        {
            var model = new StockMenuVM();
            model.SearchKey = searchKey;

            // ================= CATEGORIES =================
            model.CategoryList = _context.StockItemCategoryTables
                .Where(c => c.VisibleStatusId == 1)
                .Select(c => new FilterCategoryVM
                {
                    StockMenuCategoryID = c.StockItemCategoryId,
                    StockMenuCategory = c.StockItemCategory,
                    CategoryStatus = categoryId.HasValue && c.StockItemCategoryId == categoryId.Value
                }).ToList();

            // ================= ORDER TYPES =================
            model.OrderTypeList = _context.OrderTypeTables
                .Select(o => new FilterOrderTypeVM
                {
                    OrderTypeID = o.OrderTypeId,
                    OrderTypeName = o.OrderTypeName,
                    OrderTypeStatus = false
                }).ToList();

            // ================= ITEMS =================
            var query = _context.StockItemTables
                .Where(i => i.VisibleStatusId == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                query = query.Where(x => x.StockItemTitle.ToLower().Contains(searchKey.ToLower()));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(x => x.StockItemCategoryId == categoryId.Value);
            }

            model.AllItems = query
                .Select(item => new ItemVM
                {
                    StockItemID = item.StockItemId,
                    ItemPhotoPath = string.IsNullOrEmpty(item.ItemPhotoPath)
                        ? "/images/no-image.png"
                        : item.ItemPhotoPath,
                    StockItemTitle = item.StockItemTitle,
                    ItemSize = item.ItemSize,
                    UnitPrice = item.UnitPrice,
                    OrderTypeName = item.OrderTypeName.OrderTypeName,
                    Rating = 5
                }).ToList();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllItems(StockMenuVM model)
        {
            var query = _context.StockItemTables
                .Where(i => i.VisibleStatusId == 1)
                .AsQueryable();

            // ================= SEARCH =================
            if (!string.IsNullOrWhiteSpace(model.SearchKey))
            {
                query = query.Where(x =>
                    x.StockItemTitle.ToLower().Contains(model.SearchKey.ToLower()));
            }

            // ================= CATEGORY FILTER =================
            List<int> selectedCategories = new();
            if (model.CategoryList != null && model.CategoryList.Any())
            {
                selectedCategories = model.CategoryList
                    .Where(c => c.CategoryStatus)
                    .Select(c => c.StockMenuCategoryID)
                    .ToList();

                if (selectedCategories.Any())
                {
                    query = query.Where(x =>
                        selectedCategories.Contains(x.StockItemCategoryId));
                }
            }

            // Ensure categories are loaded for sidebar display
            var allCategories = _context.StockItemCategoryTables
                .Where(c => c.VisibleStatusId == 1)
                .Select(c => new FilterCategoryVM
                {
                    StockMenuCategoryID = c.StockItemCategoryId,
                    StockMenuCategory = c.StockItemCategory,
                    CategoryStatus = selectedCategories.Contains(c.StockItemCategoryId)
                }).ToList();
            model.CategoryList = allCategories;

            // ================= ORDER TYPE FILTER =================
            List<int> selectedOrderTypes = new();
            if (model.OrderTypeList != null && model.OrderTypeList.Any())
            {
                selectedOrderTypes = model.OrderTypeList
                    .Where(o => o.OrderTypeStatus)
                    .Select(o => o.OrderTypeID)
                    .ToList();

                if (selectedOrderTypes.Any())
                {
                    query = query.Where(x =>
                        selectedOrderTypes.Contains(x.OrderTypeId));
                }
            }

            // Ensure order types are loaded for sidebar display
            var allOrderTypes = _context.OrderTypeTables
                .Select(o => new FilterOrderTypeVM
                {
                    OrderTypeID = o.OrderTypeId,
                    OrderTypeName = o.OrderTypeName,
                    OrderTypeStatus = selectedOrderTypes.Contains(o.OrderTypeId)
                }).ToList();
            model.OrderTypeList = allOrderTypes;

            // ================= ITEMS RESULT =================
            model.AllItems = query
                .Select(item => new ItemVM
                {
                    StockItemID = item.StockItemId,
                    ItemPhotoPath = string.IsNullOrEmpty(item.ItemPhotoPath)
                        ? "/images/no-image.png"
                        : item.ItemPhotoPath,
                    StockItemTitle = item.StockItemTitle,
                    ItemSize = item.ItemSize,
                    UnitPrice = item.UnitPrice,
                    OrderTypeName = item.OrderTypeName.OrderTypeName,
                    Rating = 5
                }).ToList();

            return View(model);
        }


        [HttpGet]
        public IActionResult ProductDetail(int itemid)
        {
            if (itemid <= 0)
                return RedirectToAction("Index", "Home");

            var vm = new ItemDetailVM();
            vm.StockItemID = itemid;

            // ================= USER SESSION =================
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                var user = _context.UserTables
                    .FirstOrDefault(u => u.UserId == userId.Value);

                if (user != null)
                {
                    vm.Name = user.UserName;                 // ? keep simple
                    vm.Email = user.EmailAddress;

                    var existingReview = _context.StockItemReviewTables
                        .FirstOrDefault(r =>
                            r.StockItemID == itemid &&
                            r.ReviewBy_UserID == userId.Value);

                    if (existingReview != null)
                    {
                        vm.Rating = existingReview.Rating.ToString();
                        vm.ReviewDetails = existingReview.ReviewDetails;
                    }
                    else
                    {
                        vm.Rating = "0";
                        vm.ReviewDetails = string.Empty;
                    }
                }
            }

            // ================= ITEM DETAILS =================
            var item = _context.StockItemTables
                .Include(x => x.StockItemCategory)
                .Include(x => x.OrderTypeName)
                .FirstOrDefault(x => x.StockItemId == itemid);

            if (item == null)
                return RedirectToAction("AllItems");

            // ? IMAGE PATH FIX (NO x_)
            string imagePath = item.ItemPhotoPath;

            // ensure leading /
            if (!imagePath.StartsWith("/"))
            {
                imagePath = "/" + imagePath;
            }

            vm.Item = new ItemVM
            {
                StockItemID = item.StockItemId,
                StockItemTitle = item.StockItemTitle,
                ItemPhotoPath = imagePath,   // ? WORKING
                ItemSize = item.ItemSize,
                UnitPrice = item.UnitPrice,
                StockItemCategory = item.StockItemCategory.StockItemCategory,
                OrderTypeName = item.OrderTypeName.OrderTypeName,
                Rating = 0
            };

            // ================= REVIEWS =================
            var reviews = _context.StockItemReviewTables
    .Where(x => x.StockItemID == itemid)
    .ToList();

            if (reviews.Any())
            {
                vm.Item.Rating = (int)Math.Round(reviews.Average(x => x.Rating));
            }
            else
            {
                vm.Item.Rating = 0;
            }

            int totalReviews = 0;
            int totalRating = 0;

            foreach (var reviewRow in reviews)
            {
                totalReviews++;
                totalRating += reviewRow.Rating;

                var userData = _context.UserTables
                    .Where(u => u.UserId == reviewRow.ReviewBy_UserID)
                    .Select(u => new
                    {
                        u.UserName,
                        PhotoPath = u.UserDetailTable != null
                            ? u.UserDetailTable.PhotoPath
                            : null
                    })
                    .FirstOrDefault();

                string photoPath = userData?.PhotoPath;

                // ?? PROFILE IMAGE PATH FIX
                if (!string.IsNullOrEmpty(photoPath))
                {
                    if (!photoPath.StartsWith("/"))
                        photoPath = "/" + photoPath;
                }
                else
                {
                    photoPath = "/images/user_default.png";
                }

                vm.Reviews.Add(new ItemReviewVM
                {
                    ReviewByUser = userData?.UserName,
                    Rating = reviewRow.Rating,
                    ReviewDetails = reviewRow.ReviewDetails,
                    ReviewDays = (int)(DateTime.Now - reviewRow.ReviewDateTime).TotalDays,
                    UserPhotoPath = photoPath
                });
            }


            vm.Item.Rating = totalReviews > 0
                ? totalRating / totalReviews
                : 0;

            // ================= INGREDIENTS =================
            var ingredients = _context.StockItemIngredientTables
                .Where(i => i.StockItemId == itemid)
                .ToList();

            foreach (var ing in ingredients)
            {
                vm.Ingredients.Add(new StockItemIngredientVM
                {
                    StockItemIngredientId = ing.StockItemIngredientId,
                    StockItemIngredientTitle = ing.StockItemIngredientTitle,
                    IngredientPhotoPath = ing.StockItemIngredientPhoto,
                    StockItemId = ing.StockItemId
                });
            }

            // ================= RELATED ITEMS =================
            var related = _context.StockItemTables
                .Where(x => x.StockItemCategoryId == item.StockItemCategoryId
                            && x.StockItemId != itemid)
                .Include(x => x.OrderTypeName)
                .ToList();

            foreach (var r in related)
            {
                vm.RelatedItems.Add(new ItemVM
                {
                    StockItemID = r.StockItemId,
                    StockItemTitle = r.StockItemTitle,
                    ItemPhotoPath = r.ItemPhotoPath,
                    ItemSize = r.ItemSize,
                    UnitPrice = r.UnitPrice,
                    OrderTypeName = r.OrderTypeName.OrderTypeName
                });
            }

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductDetail(ItemDetailVM model)
        {
            int userid = HttpContext.Session.GetInt32("UserId") ?? 0;

            // ?? DEBUG CHECK
            if (userid == 0)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "User");
            }

            if (string.IsNullOrEmpty(model.Rating))
            {
                return RedirectToAction("ProductDetail", new { itemid = model.StockItemID });
            }

            int ratingValue = 5;
            if (!string.IsNullOrEmpty(model.Rating) && int.TryParse(model.Rating, out int parsedRating))
            {
                ratingValue = parsedRating;
            }

            var existingReview = _context.StockItemReviewTables
                .FirstOrDefault(x =>
                    x.StockItemID == model.StockItemID &&
                    x.ReviewBy_UserID == userid);

            if (existingReview != null)
            {
                existingReview.Rating = ratingValue;
                existingReview.ReviewDetails = model.ReviewDetails ?? "";
                existingReview.ReviewDateTime = DateTime.Now;
            }
            else
            {
                var review = new StockItemReviewTable
                {
                    StockItemID = model.StockItemID,
                    ReviewBy_UserID = userid,
                    Rating = ratingValue,
                    ReviewDetails = model.ReviewDetails ?? "",
                    ReviewDateTime = DateTime.Now
                };

                _context.StockItemReviewTables.Add(review);
            }

            _context.SaveChanges();

            return RedirectToAction("ProductDetail", new { itemid = model.StockItemID });
        }
        //About Contact page
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(FormCollection form)
        {
            // handle message
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
