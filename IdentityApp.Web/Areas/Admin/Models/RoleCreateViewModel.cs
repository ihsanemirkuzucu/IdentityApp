using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Role adı alanı boş bırakılamaz.")]
        [Display(Name = "Role Adı :")]
        public string Name{ get; set; }
    }
}
