namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Models.Account;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IDbRepository dbRepository;
        private readonly IProfileService profileService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IDbRepository dbRepository,
            IProfileService profileService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbRepository = dbRepository;
            this.profileService = profileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string userType)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (userType == null)
            {
                TempData[NotificationConstants.ErrorMessage] = "You have to pick your role in order to continue!";
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel
            {
                UserType = userType
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doesUsernameExist = await this.userManager.Users.AnyAsync(u => u.UserName == model.Username);

            if (doesUsernameExist)
            {
                this.ModelState.AddModelError(nameof(model.Username), ErrorConstants.UsernameExist);
                return View(model);
            }

            var user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var role = model.UserType == RoleConstants.Employee ? "Employee" : "Employer";

                await userManager.AddToRoleAsync(user, role);

                if (role == RoleConstants.Employee)
                {
                    var employee = new Employee
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsDeleted = false,
                        User = user
                    };

                    await this.dbRepository.AddAsync(employee);
                }
                else if (role == RoleConstants.Employer)
                {
                    var company = new Company
                    {
                        CompanyName = model.CompanyName!,
                        PhoneNumber = model.PhoneNumber!,
                        Address = model.Address!
                    };

                    await this.dbRepository.AddAsync(company);

                    var employer = new Employer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsDeleted = false,
                        User = user,
                        Company = company
                    };

                    await this.dbRepository.AddAsync(employer);
                }

                await this.dbRepository.SaveChangesAsync();
                await this.signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                model.ReturnUrl = returnUrl;
                return this.View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (user.IsDeleted)
                {
                    TempData[NotificationConstants.ErrorMessage] = "Your account has been disabled. Please contact the administrator.";
                    return View(model);
                }

                var result = await this.signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (model.ReturnUrl != null)
                    {
                        return this.Redirect(model.ReturnUrl);
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, ErrorConstants.InvalidLogin);
            return this.View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task CreateRoles()
        {
            if (!await roleManager.RoleExistsAsync(RoleConstants.Administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Administrator));
            }

            if (!await roleManager.RoleExistsAsync(RoleConstants.Employer))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Employer));
            }

            if (!await roleManager.RoleExistsAsync(RoleConstants.Employee))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Employee));
            }
        }
    }
}