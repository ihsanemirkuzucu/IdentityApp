using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Role adı alanı boş bırakılamaz.")]
        [Display(Name = "Role Adı :")]
        public string Name { get; set; } = null!;
    }
}
