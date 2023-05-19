namespace MyJobs.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models;
    using MyJobs.Core.Services;

    public class HomeController : Controller
    {
        private readonly IGetCategoriesService categoriesService;

        public HomeController(IGetCategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = categoriesService.GetCategories();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}