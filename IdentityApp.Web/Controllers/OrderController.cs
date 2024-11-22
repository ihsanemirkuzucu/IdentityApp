using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Web.Controllers
{

    public class OrderController : Controller
    {
        //[Authorize(Policy = "OrderPermissionReadOrDelete")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
