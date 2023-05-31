namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;
    using MyJobs.Core.Models.Account;

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IDbRepository dbRepository;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IDbRepository dbRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbRepository = dbRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
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
                var role = model.CompanyName != null ? "Employer" : "Employee";

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