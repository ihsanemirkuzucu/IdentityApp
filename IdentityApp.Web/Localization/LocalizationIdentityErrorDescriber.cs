using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new()
            {
                Code = "DuplicateUserName",
                Description = $"Bu kullanıcı adı: {userName} daha öncebaşka bir kullanıcı tarafından alındı."
            };
            //return base.DuplicateUserName(userName);
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new()
            {
                Code = "DuplicateEmail",
                Description = $"Bu kullanıcı adı: {email} daha öncebaşka bir kullanıcı tarafından alındı."
            };
            //return base.DuplicateEmail(email);
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az 6 karakter olamlıdır. girilen şifre uzunluğu: {length}"
            };
            // return base.PasswordTooShort(length);
        }
    }
}
