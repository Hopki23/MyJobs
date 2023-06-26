using Microsoft.AspNetCore.Mvc;

namespace MyJobs.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
