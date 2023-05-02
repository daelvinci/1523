using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Area("AdminPanel")]
    public class DashboardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
