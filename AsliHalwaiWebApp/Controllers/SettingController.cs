using AsliHalwaiWebApp.Data;
using AsliHalwaiWebApp.Models;
using AsliHalwaiWebApp.ViewModel;
using AsliHalwaiWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace AsliHalwaiWebApp.Controllers
{
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;


        public SettingController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin()
        {
            int? userTypeId = HttpContext.Session.GetInt32("UserTypeId");
            return userTypeId != null && userTypeId == 1;
        }


        private void LoadUserAddressDropdowns(UserAddressVM vm)
        {
            vm.AddressTypes = _context.AddressTypeTables
                .Select(x => new SelectListItem
                {
                    Value = x.AdressTypeId.ToString(),
                    Text = x.AddressType
                }).ToList();

            vm.VisibleStatuses = _context.VisibleStatusTables
                .Select(x => new SelectListItem
                {
                    Value = x.VisibleStatusId.ToString(),
                    Text = x.VisibleStatus
                }).ToList();
        }



        // GET: List + Edit

        public IActionResult List_UserTypes(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }

            var vm = new UserTypeVM();

            // Load list
            vm.UserTypes = _context.UserTypeTables
                .Select(x => new UserTypeVM
                {
                    UserTypeID = x.UserTypeId,
                    UserType = x.UserType
                })
                .ToList();

            // Load edit data
            if (id != null && id > 0)
            {
                var entity = _context.UserTypeTables.Find(id);
                if (entity != null)
                {
                    vm.UserTypeID = entity.UserTypeId;
                    vm.UserType = entity.UserType;
                }
            }

            return View(vm);
        }

        // POST: Create / Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult List_UserTypes(UserTypeVM model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.UserTypes = _context.UserTypeTables
                    .Select(x => new UserTypeVM
                    {
                        UserTypeID = x.UserTypeId,
                        UserType = x.UserType
                    })
                    .ToList();

                return View(model);
            }

            // CREATE
            if (model.UserTypeID == 0)
            {
                bool exists = _context.UserTypeTables
                    .Any(u => u.UserType == model.UserType);

                if (exists)
                {
                    ModelState.AddModelError("UserType", "Already Exists!");
                }
                else 
                {
                    _context.UserTypeTables.Add(new UserTypeTable
                    {
                        UserType = model.UserType
                    });
                    _context.SaveChanges();
                    return RedirectToAction(nameof(List_UserTypes));
                }
            }
            // UPDATE
            else
            {
                bool exists = _context.UserTypeTables
                    .Any(u => u.UserType == model.UserType &&
                              u.UserTypeId != model.UserTypeID);

                if (exists)
                {
                    ModelState.AddModelError("UserType", "Already Exists!");
                }
                else
                {
                    var entity = _context.UserTypeTables.Find(model.UserTypeID);
                    if (entity != null)
                    {
                        entity.UserType = model.UserType;
                        _context.SaveChanges();
                        return RedirectToAction(nameof(List_UserTypes));
                    }
                }
            }

            // Reload list if validation error
            model.UserTypes = _context.UserTypeTables
                .Select(x => new UserTypeVM
                {
                    UserTypeID = x.UserTypeId,
                    UserType = x.UserType
                })
                .ToList();

            return View(model);
        }

        // DELETE (extra – client ready)
        [HttpPost]
        public IActionResult DeleteUserType(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var entity = _context.UserTypeTables.Find(id);
            if (entity != null)
            {
                _context.UserTypeTables.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List_UserTypes));
        }
    
    // GENDER : LIST + EDIT
        // =======================
        public IActionResult List_Genders(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var vm = new GenderVM();

            // Load list
            vm.Genders = _context.GenderTables
                .Select(g => new GenderVM
                {
                    GenderID = g.GenderId,
                    GenderTitle = g.Gender
                })
                .ToList();

            // Load edit data
            if (id != null && id > 0)
            {
                var entity = _context.GenderTables.Find(id);
                if (entity != null)
                {
                    vm.GenderID = entity.GenderId;
                    vm.GenderTitle = entity.Gender;
                }
            }

            return View(vm);
        }

        // =======================
        // GENDER : CREATE / UPDATE
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult List_Genders(GenderVM model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.Genders = _context.GenderTables
                    .Select(g => new GenderVM
                    {
                        GenderID = g.GenderId,
                        GenderTitle = g.Gender
                    })
                    .ToList();

                return View(model);
            }

            // CREATE
            if (model.GenderID == 0)
            {
                bool exists = _context.GenderTables
                    .Any(g => g.Gender == model.GenderTitle);

                if (exists)
                {
                    ModelState.AddModelError("GenderTitle", "Already Exists!");
                }
                else
                {
                    _context.GenderTables.Add(new GenderTable
                    {
                        Gender = model.GenderTitle
                    });
                    _context.SaveChanges();
                    return RedirectToAction(nameof(List_Genders));
                }
            }
            // UPDATE
            else
            {
                bool exists = _context.GenderTables
                    .Any(g => g.Gender == model.GenderTitle &&
                              g.GenderId != model.GenderID);

                if (exists)
                {
                    ModelState.AddModelError("GenderTitle", "Already Exists!");
                }
                else
                {
                    var entity = _context.GenderTables.Find(model.GenderID);
                    if (entity != null)
                    {
                        entity.Gender = model.GenderTitle;
                        _context.SaveChanges();
                        return RedirectToAction(nameof(List_Genders));
                    }
                }
            }

            // Reload list on error
            model.Genders = _context.GenderTables
                .Select(g => new GenderVM
                {
                    GenderID = g.GenderId,
                    GenderTitle = g.Gender
                })
                .ToList();

            return View(model);
        }

        // =======================
        // GENDER : DELETE (Extra)
        // =======================
        [HttpPost]
        public IActionResult DeleteGender(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var entity = _context.GenderTables.Find(id);
            if (entity != null)
            {
                _context.GenderTables.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List_Genders));
        }
    
     // USER STATUS : LIST + EDIT
        // =========================
        public IActionResult List_UserStatus(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var vm = new UserStatusVM();

            // Load list
            vm.UserStatuses = _context.UserStatusTables
                .Select(s => new UserStatusVM
                {
                    UserStatusID = s.UserStatusId,          // DB -> VM mapping
                    UserStatus = s.UserStatus
                })
                .ToList();

            // Load edit data
            if (id.HasValue && id > 0)
            {
                var entity = _context.UserStatusTables.Find(id.Value);
                if (entity != null)
                {
                    vm.UserStatusID = entity.UserStatusId;
                    vm.UserStatus = entity.UserStatus;
                }
            }

            return View(vm);
        }

        // =========================
        // USER STATUS : CREATE / UPDATE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult List_UserStatus(UserStatusVM model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.UserStatuses = _context.UserStatusTables
                    .Select(s => new UserStatusVM
                    {
                        UserStatusID = s.UserStatusId,
                        UserStatus = s.UserStatus
                    })
                    .ToList();

                ModelState.AddModelError(string.Empty, "Fill All Field Properly.");
                return View(model);
            }

            // CREATE
            if (model.UserStatusID == 0)
            {
                bool exists = _context.UserStatusTables
                    .Any(s => s.UserStatus.ToUpper() == model.UserStatus.ToUpper());

                if (exists)
                {
                    ModelState.AddModelError("UserStatus", "Already Exists!");
                }
                else
                {
                    _context.UserStatusTables.Add(new UserStatusTable
                    {
                        UserStatus = model.UserStatus
                    });
                    _context.SaveChanges();
                    return RedirectToAction(nameof(List_UserStatus));
                }
            }
            // UPDATE
            else
            {
                bool exists = _context.UserStatusTables
                    .Any(s => s.UserStatus.ToUpper() == model.UserStatus.ToUpper()
                           && s.UserStatusId != model.UserStatusID);

                if (exists)
                {
                    ModelState.AddModelError("UserStatus", "Already Exists!");
                }
                else
                {
                    var entity = _context.UserStatusTables.Find(model.UserStatusID);
                    if (entity != null)
                    {
                        entity.UserStatus = model.UserStatus;
                        _context.SaveChanges();
                        return RedirectToAction(nameof(List_UserStatus));
                    }
                }
            }

            // Reload list if error
            model.UserStatuses = _context.UserStatusTables
                .Select(s => new UserStatusVM
                {
                    UserStatusID = s.UserStatusId,
                    UserStatus = s.UserStatus
                })
                .ToList();

            return View(model);
        }

        // =========================
        // USER STATUS : DELETE
        // =========================
        [HttpPost]
        public IActionResult DeleteUserStatus(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var entity = _context.UserStatusTables.Find(id);
            if (entity != null)
            {
                _context.UserStatusTables.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List_UserStatus));
        }



    
    // VISIBLE STATUS : LIST + EDIT (GET)
        // ==================================
        public IActionResult List_VisibleStatus(int? id)
        {
            var vm = new VisibleStatusVM();

            // Load list
            vm.VisibleStatuses = _context.VisibleStatusTables
                .Select(v => new VisibleStatusVM
                {
                    VisibleStatusID = v.VisibleStatusId,        // DB → VM
                    VisibleStatus = v.VisibleStatus
                })
                .ToList();

            // Load edit data
            if (id.HasValue && id > 0)
            {
                var entity = _context.VisibleStatusTables.Find(id.Value);
                if (entity != null)
                {
                    vm.VisibleStatusID = entity.VisibleStatusId;
                    vm.VisibleStatus = entity.VisibleStatus;
                }
            }

            return View(vm);
        }

        // ==================================
        // VISIBLE STATUS : CREATE / UPDATE (POST)
        // ==================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult List_VisibleStatus(VisibleStatusVM model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.VisibleStatuses = _context.VisibleStatusTables
                    .Select(v => new VisibleStatusVM
                    {
                        VisibleStatusID = v.VisibleStatusId,
                        VisibleStatus = v.VisibleStatus
                    })
                    .ToList();

                ModelState.AddModelError(string.Empty, "Fill All Field Properly.");
                return View(model);
            }

            // CREATE
            if (model.VisibleStatusID == 0)
            {
                bool exists = _context.VisibleStatusTables
                    .Any(v => v.VisibleStatus.ToUpper() == model.VisibleStatus.ToUpper());

                if (exists)
                {
                    ModelState.AddModelError("VisibleStatus", "Already Exists!");
                }
                else
                {
                    _context.VisibleStatusTables.Add(new VisibleStatusTable
                    {
                        VisibleStatus = model.VisibleStatus
                    });
                    _context.SaveChanges();
                    return RedirectToAction(nameof(List_VisibleStatus));
                }
            }
            // UPDATE
            else
            {
                bool exists = _context.VisibleStatusTables
                    .Any(v => v.VisibleStatus.ToUpper() == model.VisibleStatus.ToUpper()
                           && v.VisibleStatusId != model.VisibleStatusID);

                if (exists)
                {
                    ModelState.AddModelError("VisibleStatus", "Already Exists!");
                }
                else
                {
                    var entity = _context.VisibleStatusTables.Find(model.VisibleStatusID);
                    if (entity != null)
                    {
                        entity.VisibleStatus = model.VisibleStatus;
                        _context.SaveChanges();
                        return RedirectToAction(nameof(List_VisibleStatus));
                    }
                }
            }

            // Reload list on error
            model.VisibleStatuses = _context.VisibleStatusTables
                .Select(v => new VisibleStatusVM
                {
                    VisibleStatusID = v.VisibleStatusId,
                    VisibleStatus = v.VisibleStatus
                })
                .ToList();

            return View(model);
        }

        // ==================================
        // VISIBLE STATUS : DELETE
        // ==================================
        [HttpPost]
        public IActionResult DeleteVisibleStatus(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
                // OR
                // return Unauthorized();
            }
            var entity = _context.VisibleStatusTables.Find(id);
            if (entity != null)
            {
                _context.VisibleStatusTables.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(List_VisibleStatus));
        }



        [HttpGet]
        public IActionResult List_UserAddress(int? id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var vm = new UserAddressVM();
            LoadUserAddressDropdowns(vm);

            if (id.HasValue)
            {
                var address = _context.UserAddressTables
                    .FirstOrDefault(x => x.UserAddressId == id && x.UserId == userId);

                if (address != null)
                {
                    vm.UserAddressID = address.UserAddressId;
                    vm.AddressTypeID = address.AddressTypeId;
                    vm.FullAddress = address.FullAddress;
                    vm.VisibleStatusID = address.VisibleStatusId;
                }
            }

            vm.AddressList = _context.UserAddressTables
                .Where(x => x.UserId == userId)
                .Select(x => new UserAddressListVM
                {
                    UserAddressID = x.UserAddressId,
                    AddressType = x.AddressType.AddressType,
                    FullAddress = x.FullAddress,
                    VisibleStatus = x.VisibleStatus.VisibleStatus,
                    UserName = x.User.UserName
                }).ToList();

            return View(vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult List_UserAddress(UserAddressVM vm)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
            {
                LoadUserAddressDropdowns(vm);
                return View(vm);
            }

            if (vm.UserAddressID == 0)
            {
                _context.UserAddressTables.Add(new UserAddressTable
                {
                    UserId = userId.Value,
                    AddressTypeId = vm.AddressTypeID,
                    FullAddress = vm.FullAddress,
                    VisibleStatusId = vm.VisibleStatusID,
                    CreatedByUserId = userId.Value
                });
            }
            else
            {
                var address = _context.UserAddressTables
                    .FirstOrDefault(x => x.UserAddressId == vm.UserAddressID);

                if (address != null)
                {
                    address.AddressTypeId = vm.AddressTypeID;
                    address.FullAddress = vm.FullAddress;
                    address.VisibleStatusId = vm.VisibleStatusID;
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(List_UserAddress));
        }





    }

}








