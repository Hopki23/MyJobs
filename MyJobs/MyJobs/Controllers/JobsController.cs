namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Infrastructure.Models;

    public class JobsController : Controller
    {
        private readonly IGetCategoriesService categoriesService;
        private readonly IDbRepository repository;
        private readonly IJobService jobService;

        public JobsController(
            IGetCategoriesService categoriesService,
            IDbRepository repository,
            IJobService jobService)
        {
            this.categoriesService = categoriesService;
            this.repository = repository;
            this.jobService = jobService;
        }

        [HttpGet]
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            var model = new CreateJobViewModel
            {
                CategoryItems = this.categoriesService.GetAllCategories(),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public IActionResult Create(CreateJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryItems = this.categoriesService.GetAllCategories();
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = this.repository.All<Employer>()
                .Where(e => e.UserId == userId)
                .Select(e => new { e.EmployerId, e.CompanyId })
                .FirstOrDefault();

            int employerId = result.EmployerId;
            int companyId = result.CompanyId;

            this.jobService.CreateAsync(model, employerId, companyId);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult All(int page = 1)
        {
            const int ItemsPerPage = 5;
            var model = new JobsListViewModel()
            {
                PageNumber = page,
                ItemsPerPage = ItemsPerPage,
                Jobs = this.jobService.GetAllJobs(page, ItemsPerPage),
                JobsTotalCount = this.jobService.GetTotalJobCount()
            };

            return View(model);
        }
    }
}
