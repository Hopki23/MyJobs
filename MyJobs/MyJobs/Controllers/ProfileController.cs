namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Models.Profile;
    using MyJobs.Core.Services.Contracts;

    public class ProfileController : BaseController
    {
        private readonly IProfileService profileService;
        private readonly IDbRepository repository;
        private readonly IJobService jobService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ProfileController(
            IProfileService profileService,
            IDbRepository repository,
            IJobService jobService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.profileService = profileService;
            this.repository = repository;
            this.jobService = jobService;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
                var userProfile = this.profileService.GetUserById(user.Id, role);

                ViewBag.UserId = userProfile.Id;

                return PartialView("_PersonalInformationPartial", userProfile);
            }
            catch (Exception)
            {
                //Add error.
                ModelState.AddModelError(string.Empty, "");

                return View();
            }

        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> MyJobs()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employer = await this.repository.AllReadonly<Employer>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employer == null)
            {
                throw new ArgumentException("Employer not found!");
            }

            var jobs = await this.jobService.GetJobsForCertainEmployer(employer);

            return PartialView("_MyJobsPartial", jobs);
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> MyApplications()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                throw new ArgumentException("Employee not found!");
            }

            var jobs = await this.jobService.GetJobsByEmployeeId(employee.Id);

            return PartialView("_MyApplicationsPartial", jobs);
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> Notifications()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return NotFound();
            }

            var notifications = await this.profileService.GetUnreadNotificationsForEmployee(employee.Id);

            return PartialView("_MyNotificationsPartial", notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await this.profileService.MarkNotificationAsRead(notificationId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var model = await this.profileService.GetProfileForEditing(id, userId);
                model.Id = id;
                return View(model);
            }
            catch (Exception)
            {
                //TODO: implement error handling
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(int id, UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound();
            }

            try
            {
                await this.profileService.EditProfile(model, id, userId);
                return RedirectToAction(nameof(Index), new { id });
            }
            catch (Exception)
            {
                //TODO: implement error handling
                throw;
            }

        }
    }
}
