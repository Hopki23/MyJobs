namespace MyJobs.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models;

    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await this.categoryService.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Category category = new()
            {
                Name = model.Name
            };

            await this.categoryService.AddCategoryAsync(category);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //First delete the jobs
                await this.categoryService.DeleteCategoryJobsAsync(id);

                //Then the category
                await this.categoryService.DeleteCategoryAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await this.categoryService.RestoreCategoryJobsAsync(id);

                await this.categoryService.RestoreCategoryAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}
