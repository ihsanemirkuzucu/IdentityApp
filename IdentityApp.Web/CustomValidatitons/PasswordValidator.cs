using IdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.CustomValidatitons
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez." });
            }
            if (password!.ToLower().StartsWith("123"))
            {
                errors.Add(new() { Code = "PasswordContain123", Description = "Şifre alanı 123 ile başlayamaz." });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
