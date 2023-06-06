namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MyJobs.Core.Services;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class ProfileController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProfileService profileService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IProfileService profileService)
        {
            this.userManager = userManager;
            this.profileService = profileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PersonalInformation()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == null)
            {
                return NotFound();
            }

            try
            {
                var userProfile = this.profileService.GetUserById(user.Id, role);
                return PartialView("_PersonalInformationPartial", userProfile);
            }
            catch (Exception)
            {
                //TODO: Return custom error view();
                throw;
            }

        }
    }
}
