using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Eski Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Eski Şifre :")]
        public string PasswordOld { get; set; }

        [Required(ErrorMessage = "Yeni Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre :")]
        [MinLength(6,ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string PasswordNew { get; set; }

        [Compare(nameof(PasswordNew), ErrorMessage = "Girilen şifreler aynı değildir.")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar :")]
        public string PasswordNewConfirm { get; set; }
    }
}
