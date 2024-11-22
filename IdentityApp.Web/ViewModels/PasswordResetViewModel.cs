using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
    public class PasswordResetViewModel
    {
        //[DataType(DataType.Password)] css deki input type passworde denktir.
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre :")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Girilen şifreler aynı değildir.")]
        [Required(ErrorMessage = "Şifre Tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}
