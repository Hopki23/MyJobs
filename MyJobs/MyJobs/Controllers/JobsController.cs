namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;

    public class JobsController : Controller
    {
        private readonly ICategoryService categoriesService;
        private readonly IDbRepository repository;
        private readonly IJobService jobService;

        public JobsController(
            ICategoryService categoriesService,
            IDbRepository repository,
            IJobService jobService)
        {
            this.categoriesService = categoriesService;
            this.repository = repository;
            this.jobService = jobService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Create()
        {
            var model = new CreateJobViewModel
            {
                CategoryItems = await this.categoriesService.GetAllCategories(),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryItems = await this.categoriesService.GetAllCategories();
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await this.repository.All<Employer>()
                .Where(e => e.UserId == userId)
                .Select(e => new { e.Id, e.CompanyId })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return View(nameof(All));
            }

            int employerId = result.Id;
            int companyId = result.CompanyId;

            try
            {
                await this.jobService.CreateAsync(model, employerId, companyId);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return View("CustomError");
            }

        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1)
        {
            const int ItemsPerPage = 7;
            var filterViewModel = await this.jobService.GetJobFilterViewModel();

            var model = new JobsListViewModel
            {
                PageNumber = page,
                ItemsPerPage = ItemsPerPage,
                Jobs = await this.jobService.GetAllJobs(page, ItemsPerPage),
                JobsTotalCount = this.jobService.GetTotalJobCount(),
                JobFilter = filterViewModel,
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = await this.repository.All<Employer>()
                                        .FirstOrDefaultAsync(e => e.UserId == userId);

            var model = await this.jobService.GetSingleJob(id, employer);
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

            var employee = await this.repository.All<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return RedirectToAction(nameof(All));
            }

            try
            {
                await this.jobService.Apply(model, employee);
                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> ReceivedResumes(JobsWithCVsViewModel model)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = await this.repository.All<Employer>()
                            .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                return RedirectToAction(nameof(All));
            }

            try
            {
                var jobViewModels = await this.jobService.GetJobsWithCV(model, employer);

                return View(jobViewModels);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var model = await this.jobService.GetById(id, userId);
                model.CategoryItems = await this.categoriesService.GetAllCategories();

                return View(model);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
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
                return View("CustomError");
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
                return View("CustomError");
            }

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public IActionResult Filter(string select, string[] selectedWorkingTimes, string locationSelect)
        {
            try
            {
                var filteredJobOffers = this.jobService.FilterJobOffers(select, selectedWorkingTimes, locationSelect);

                return PartialView("_FilteredJobOffersPartial", filteredJobOffers);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}