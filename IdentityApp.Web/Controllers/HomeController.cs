using IdentityApp.Web.CustomExtensions;
using IdentityApp.Web.Models;
using IdentityApp.Web.Services;
using IdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace IdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home");
            var hasUser = await _userManager.FindByEmailAsync(request.Email);
            if (hasUser is null)
            {
                ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ.");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriþ yapamazsýnýz" });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                var accessFailedCount = await _userManager.GetAccessFailedCountAsync(hasUser);
                ModelState.AddModelErrorList(new List<string>() { "Email veya þifre yanlýþ.", $"Baþarýsýz giriþ sayýsý: {accessFailedCount}" });
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe,
                    new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
            }

            return Redirect(returnUrl!);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SigUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var idetityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email
            }, request.PasswordConfirm);

            if (!idetityResult.Succeeded)
            {
                ModelState.AddModelErrorList(idetityResult.Errors);
                return View();
            }

            var exchangeClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(30).ToString());
            var currentUser = (await _userManager.FindByNameAsync(request.UserName))!;
            var claimResult = await _userManager.AddClaimAsync(currentUser, exchangeClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Üyelik kayýt iþlemi baþarýyla gerçekleþtirilmiþtir.";
            return RedirectToAction(nameof(HomeController.SignUp));
        }

        public IActionResult ResetPasword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasword(ResetPasswordViewModel request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.Email);
            if (hasUser is null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanýcý bulunamamýþtýr.");
                return View();
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("PasswordReset", "Home",
                new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);
            //örnek link:https://localhost:7137
            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Þifre yenileme linki e-posta adresinize gönderilmiþtir.";
            return RedirectToAction(nameof(ResetPasword));
        }

        public IActionResult PasswordReset(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId is null && token is null)
            {
                throw new Exception("Hata meydana geldi");
            }
            var hasUser = await _userManager.FindByIdAsync(userId!.ToString()!);
            if (hasUser is null)
            {
                ModelState.AddModelError(string.Empty, "Kullanýcý bulunamadý.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token!.ToString()!, request.PasswordConfirm);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifreniz baþarýyla yenilenmiþtir.";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors);
            }
            return View();
        }











        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
