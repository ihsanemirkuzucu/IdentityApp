using IdentityApp.Web.Areas.Admin.Models;
using IdentityApp.Web.CustomExtensions;
using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();
            return View(roles);
        }

        [Authorize(Roles = "RoleAction")]
        public IActionResult RoleAdd()
        {
            return View();
        }

        [Authorize(Roles = "RoleAction")]
        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol Başarı ile Eklendi.";
            return RedirectToAction(nameof(RoleController.Index));
        }

        [Authorize(Roles = "RoleAction")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate is null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate!.Id, Name = roleToUpdate.Name! });
        }

        [Authorize(Roles = "RoleAction")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate is null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır");
            }

            roleToUpdate.Name = request.Name;
            var updatedRole = await _roleManager.UpdateAsync(roleToUpdate);
            if (!updatedRole.Succeeded)
            {
                ModelState.AddModelErrorList(updatedRole.Errors);
                return View();
            }

            ViewData["SuccessMessage"] = "Role başarıyla güncellenmiştir";
            return View();
        }

        [Authorize(Roles = "RoleAction")]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete is null)
            {
                throw new Exception("Güncellenecek role bulunamamıştır");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Role Silinmiştir";
            return RedirectToAction(nameof(RoleController.Index));
        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!;
            ViewBag.UserId = id;
            var roles = await _roleManager.Roles.ToListAsync();
            var roleViewModelList = new List<AssignRoleToUserViewModel>();
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> request)
        {
            var currenUser = (await _userManager.FindByIdAsync(userId))!;
            foreach (var role in request)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(currenUser, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(currenUser, role.Name);
                }
            }

            TempData["SuccessMessage"] = "Rol Güncelleme işlemi başarılı şekilde yapıldı";
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
