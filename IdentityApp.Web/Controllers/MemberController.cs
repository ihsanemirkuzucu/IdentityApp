using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using IdentityApp.Web.CustomExtensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace IdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var userViewModel = new UserViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };

            return View(userViewModel);
        }

        public IActionResult ParwordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ParwordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var curentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var checkOldPassword = await _userManager.CheckPasswordAsync(curentUser, request.PasswordOld);
            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlıştır!");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(curentUser, request.PasswordOld, request.PasswordNewConfirm);
            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList((resultChangePassword.Errors));
                return View();
            }
            //hassas bilgiler güncellendiğinde securitystampp değeri güncellenmelidir!
            await _userManager.UpdateSecurityStampAsync(curentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(curentUser, request.PasswordNewConfirm, true, false);

            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellenmiştir.";
            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.GenderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            var userEditViewModel = new UserEditViwModel
            {
                UserName = currentUser.UserName!,
                BirthDay = currentUser.BirthDate,
                City = currentUser.City,
                Email = currentUser.Email!,
                Gender = currentUser.Gender,
                Phone = currentUser.PhoneNumber!,
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViwModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.Gender = request.Gender;
            currentUser.BirthDate = request.BirthDay;
            currentUser.City = request.City;
            currentUser.PhoneNumber = request.Phone;

            if (request.Picture is not null && request.Picture.Length > 0)
            {
                var wwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";
                var newPicturePath = Path.Combine(wwrootFolder.First(x => x.Name == "UserPictures").PhysicalPath!,
                    randomFileName);
                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await request.Picture.CopyToAsync(stream);
                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            if (request.BirthDay.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true,
                    new[] { new Claim("birthdate", currentUser.BirthDate.Value.ToString()) });
                TempData["SuccessMessage"] = "Üye bilgileri başarıile güncellendi";
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }
            var userEditViewModel = new UserEditViwModel
            {
                UserName = currentUser.UserName!,
                BirthDay = currentUser.BirthDate,
                City = currentUser.City,
                Email = currentUser.Email!,
                Gender = currentUser.Gender,
                Phone = currentUser.PhoneNumber!,
            };

            return View(userEditViewModel);
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            var message = string.Empty;
            message = "Bu sayfayı görüntülemek için yetkiniz yoktur. Yetki almak için yöneticiniz ile görüşün";

            ViewBag.Message=message;
            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            var userClaimList = User.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
            return View(userClaimList);
        }

        [Authorize(Policy = "IstanbulPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolancePolicy")]
        [HttpGet]
        public IActionResult ViolancePage()
        {
            return View();
        }
    }
}
