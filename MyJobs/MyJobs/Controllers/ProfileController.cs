namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;

    public class ProfileController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProfileService profileService;
        private readonly IDbRepository repository;
        private readonly IJobService jobService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService,
            IDbRepository repository,
            IJobService jobService)
        {
            this.userManager = userManager;
            this.profileService = profileService;
            this.repository = repository;
            this.jobService = jobService;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                throw new ArgumentException("Employee not found!");
            }

            var jobs = await this.jobService.GetJobsByEmployeeId(employee.Id);

            return PartialView("_MyApplicationsPartial", jobs);
        }
    }
}
