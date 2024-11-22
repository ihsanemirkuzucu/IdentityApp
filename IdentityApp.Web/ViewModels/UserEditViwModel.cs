using System.ComponentModel.DataAnnotations;
using IdentityApp.Web.Models;

namespace IdentityApp.Web.ViewModels
{
    public class UserEditViwModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı hatalı.")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; }


        [Display(Name = "Şehir :")]
        public string? City { get; set; }

        [Display(Name = "Profil Fotoğrafı :")]
        public IFormFile? Picture { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi :")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "Cinsiyet :")]
        public Gender? Gender { get; set; }

    }
}
