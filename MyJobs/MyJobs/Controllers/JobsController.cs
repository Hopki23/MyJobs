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
            bool doesCategoryExist = await this.categoriesService.CategoryExistById(model.CategoryId);

            if (!doesCategoryExist)
            {
                TempData[NotificationConstants.ErrorMessage] = "Selected category does not exist!";
                model.CategoryItems = await this.categoriesService.GetAllCategories();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.CategoryItems = await this.categoriesService.GetAllCategories();
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                await this.jobService.CreateAsync(model, userId);
                TempData[NotificationConstants.InformationMessage] = "Your job offer is under review by the admin. We will notify you once a decision has been made.";
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
                JobsTotalCount = await this.jobService.GetTotalJobCount(),
                JobFilter = filterViewModel,
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var model = await this.jobService.GetSingleJob(id, userId!);
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

            try
            {
                await this.jobService.Apply(model, userId);
                TempData[NotificationConstants.SuccessMessage] = NotificationConstants.SuccessApply;
                return RedirectToAction(nameof(All));
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == NotificationConstants.CreateResumeError)
                {
                    TempData[NotificationConstants.ErrorMessage] = ex.Message;

                    return RedirectToAction("Create", "Resume");
                }
                else if (ex.Message == NotificationConstants.AlreadyApprovedMessageError)
                {
                    TempData[NotificationConstants.ErrorMessage] = ex.Message;

                    return RedirectToAction(nameof(All));
                }

                return View("CustomError");
            }
            catch (InvalidOperationException ex)
            {
                TempData[NotificationConstants.ErrorMessage] = ex.Message;

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> ReceivedResumes(JobsWithCVsViewModel model)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var jobViewModels = await this.jobService.GetJobsWithCV(model, userId!);

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
                var model = await this.jobService.GetById(id, userId!);
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
                return RedirectToAction(nameof(GetById), new { id });
            }
            catch (Exception)
            {
                return View("CustomError");
            }
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