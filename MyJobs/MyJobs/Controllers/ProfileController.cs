namespace MyJobs.Controllers
{
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Core.Models.Profile;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Models.Resume;

    public class ProfileController : BaseController
    {
        private readonly IProfileService profileService;
        private readonly IJobService jobService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IResumeService resumeService;
        private readonly IDbRepository repository;

        public ProfileController(
            IProfileService profileService,
            IJobService jobService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IResumeService resumeService,
            IDbRepository repository)
        {
            this.userManager = userManager;
            this.profileService = profileService;
            this.jobService = jobService;
            this.roleManager = roleManager;
            this.resumeService = resumeService;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> PersonalInformation()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await this.userManager.FindByIdAsync(userId);

            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == null)
            {
                throw new ArgumentException("Invalid role!");
            }

            try
            {
                var userProfile = await this.profileService.GetUserById(user.Id, role);

                //ViewBag.UserId = userProfile.Id;

                return View(userProfile);
            }
            catch (Exception)
            {
                return View("CustomError");
            }

        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> MyJobs()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var jobs = await this.jobService.GetJobsForCertainEmployer(userId);

            if (jobs != null)
            {
                return View(jobs);
            }

            return View("CustomError");
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> MyApplications()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var jobs = await this.jobService.GetJobsByEmployeeId(userId);

            if (jobs != null)
            {
                return View(jobs);
            }

            return View("CustomError");
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> Notifications()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                var notifications = await this.profileService.GetUnreadNotifications(userId);
                return Json(notifications);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await this.profileService.MarkNotificationAsRead(id);

                return RedirectToAction(nameof(Notifications));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                var model = await this.profileService.GetProfileForEditing(id, userId!);
                model.Id = id;
                return View(model);
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(int id, UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                await this.profileService.EditProfile(model, id, userId);
                return RedirectToAction(nameof(PersonalInformation));
            }
            catch (Exception)
            {
                return View("CustomError");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ReadNotifications()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var notifications = await this.profileService.GetReadNotifications(userId);

            if (notifications != null)
            {
                return View(notifications);
            }

            return View("CustomError");
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> MyResumes()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var resumes = await this.resumeService.MyResumes(userId);

            return View(resumes);
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]

        public async Task<IActionResult> DownloadResumeAsync(int cvId)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                var resume = await this.resumeService.DownloadResume(userId, cvId);
                return resume;
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> EditResume(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            try
            {
                var resume = await this.resumeService.GetResumeForEdit(userId, id);
                return View(resume);
            }
            catch (ArgumentException ex)
            {
                TempData[NotificationConstants.ErrorMessage] = ex.Message;
                return RedirectToAction(nameof(MyResumes));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employee)]

        public async Task<IActionResult> EditResume(int id, EditResumeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await this.resumeService.Update(id, model);
                return RedirectToAction(nameof(MyResumes));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
               await this.resumeService.Delete(id);
            }
            catch (Exception)
            {
                return View("CustomError");
            }

            return RedirectToAction(nameof(MyResumes));
        }
    }
}
