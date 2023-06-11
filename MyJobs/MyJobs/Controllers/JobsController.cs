namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
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
        [Authorize(Roles = RoleConstants.Employer)]
        public IActionResult Create()
        {
            var model = new CreateJobViewModel
            {
                CategoryItems = this.categoriesService.GetAllCategories(),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public IActionResult Create(CreateJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryItems = this.categoriesService.GetAllCategories();
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = this.repository.All<Employer>()
                .Where(e => e.UserId == userId)
                .Select(e => new { e.Id, e.CompanyId })
                .FirstOrDefault();

            int employerId = result!.Id;
            int companyId = result.CompanyId;

            this.jobService.CreateAsync(model, employerId, companyId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult All(int page = 1)
        {
            const int ItemsPerPage = 7;
            var filterViewModel = this.jobService.GetJobFilterViewModel();

            var model = new JobsListViewModel
            {
                PageNumber = page,
                ItemsPerPage = ItemsPerPage,
                Jobs = this.jobService.GetAllJobs(page, ItemsPerPage),
                JobsTotalCount = this.jobService.GetTotalJobCount(),
                JobFilter = filterViewModel,

            };
            return View(model);
        }


        [HttpGet]
        public IActionResult GetById(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = this.repository.All<Employer>()
                                        .FirstOrDefault(e => e.UserId == userId);

            var model = this.jobService.GetSingleJob(id, employer);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Apply(int id)
        {
            var model = new UploadResumeViewModel()
            {
                Id = id
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Apply(UploadResumeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var employee = this.repository.All<Employee>()
                .FirstOrDefault(e => e.UserId == userId);

            if (employee == null)
            {
                return RedirectToAction(nameof(All));
            }

            try
            {
                await this.jobService.Apply(model, employee);
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public IActionResult ReceivedResumes(JobsWithCVsViewModel model)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = this.repository.All<Employer>()
                            .FirstOrDefault(e => e.UserId == userId);

            if (employer == null)
            {
                return RedirectToAction(nameof(All));
            }

            var jobViewModels = this.jobService.GetJobsWithCV(model, employer);

            return View(jobViewModels);
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public IActionResult Edit(int id)
        {
            var model = this.jobService.GetById(id);
            model.CategoryItems = this.categoriesService.GetAllCategories();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Edit(int id, EditJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await this.jobService.Update(id, model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred");
            }

            return RedirectToAction(nameof(GetById), new { id });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await this.jobService.Delete(id);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred");
            }

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public IActionResult Filter(string select, string[] selectedWorkingTimes, string locationSelect)
        {
            var filteredJobOffers = this.jobService.FilterJobOffers(select, selectedWorkingTimes, locationSelect);

            return PartialView("_FilteredJobOffersPartial", filteredJobOffers);
        }
    }
}