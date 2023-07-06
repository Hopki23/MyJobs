﻿namespace MyJobs.Controllers
{
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache cache;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ProfileController(
            IProfileService profileService,
            IDbRepository repository,
            IJobService jobService,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.profileService = profileService;
            this.repository = repository;
            this.jobService = jobService;
            this.cache = cache;
            this.roleManager = roleManager;
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

                ViewBag.UserId = userProfile.Id;

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
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employer = await this.repository.AllReadonly<Employer>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            var jobs = await this.jobService.GetJobsForCertainEmployer(employer!);

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
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            var jobs = await this.jobService.GetJobsByEmployeeId(employee!.Id);

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
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            try
            {
                var notifications = await this.profileService.GetUnreadNotifications(employee!.Id);
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

        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var model = await this.profileService.GetProfileForEditing(id, userId);
                model.Id = id;
                return View(model);
            }
            catch (Exception)
            {
                return View("CustomError");
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
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await this.profileService.EditProfile(model, id, userId);
                return RedirectToAction(nameof(Index), new { id });
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
            var employee = await this.repository.AllReadonly<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            var notifications = await this.profileService.GetReadNotifications(employee!.Id);

            if (notifications != null)
            {
                return View(notifications);
            }

            return View("CustomError");
        }
    }
}
