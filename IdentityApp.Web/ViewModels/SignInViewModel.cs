using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
        }
        public SignInViewModel(string email, string password) : base()
        {
            Email = email;
            Password = password;
        }

        [EmailAddress(ErrorMessage = "Email formatı hatalı.")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
