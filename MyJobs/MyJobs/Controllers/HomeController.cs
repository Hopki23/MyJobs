namespace MyJobs.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Error;
    using MyJobs.Core.Services.Contracts;

    public class HomeController : Controller
    {
        private readonly IGetCategoriesService categoriesService;

        public HomeController(IGetCategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await this.categoriesService.GetCategories();
                return View(viewModel);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}