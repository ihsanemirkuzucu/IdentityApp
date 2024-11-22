using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email formatı hatalı.")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email :")]
        public string Email { get; set; }
    }
}
