using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsliHalwaiWebApp.Controllers
{

    public class MenuController : Controller
    {
        private readonly AppDbContext _context;


        public MenuController(AppDbContext context)
        {
            _context = context;
        }

       

        public IActionResult Menu()
        {
            var vm = new CustomerMenuVM();

            var categories = _context.StockItemCategoryTables.ToList();

            foreach (var cat in categories)
            {
                var categoryVm = new CustomerMenuCategoryVM
                {
                    StockItemCategoryId = cat.StockItemCategoryId,
                    StockCategory = cat.StockItemCategory
                };

                categoryVm.Items = _context.StockItemTables
                    .Where(x => x.StockItemCategoryId == cat.StockItemCategoryId
                             && x.VisibleStatus.VisibleStatus == "Visible")
                    .Select(x => new CustomerStockItemVM
                    {
                        StockItemId = x.StockItemId,
                        StockItemTitle = x.StockItemTitle,
                        ItemPhotoPath = x.ItemPhotoPath,
                        ItemSize = x.ItemSize,
                        UnitPrice = x.UnitPrice,
                        OrderType = x.OrderTypeName.OrderTypeName,
                        VisibleStatus = x.VisibleStatus.VisibleStatus
                    }).ToList();

                vm.Categories.Add(categoryVm);
            }

            return View(vm);
        }

    }
}
