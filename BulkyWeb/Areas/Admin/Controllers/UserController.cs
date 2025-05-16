using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        public readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        // we use this to access the wwwroot folder
        public UserController(ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;

        }

        public IActionResult Index()
        {
           return View();
        }

        [HttpGet]
        public IActionResult RoleManagment(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _db.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefault(u => u.Id == userId),

                // Projection 
                RoleList = _db.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                }),
                // Projection
                CompanyList = _db.Companies.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })

            };
            // we need to set the role of the user
            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
            return View(RoleVM);
        }
        [HttpPost]
        public IActionResult RoleManagment(RoleManagementVM roleManagementVM)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in objUserList)
            {
                var roleid = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleid).Name;
                

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked and we need to unlock them 
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        
        #endregion

    }
}
