namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Models;

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return this.View(model);
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
                this.ModelState.AddModelError(nameof(model.Username), "Username already exists!");
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

            this.ModelState.AddModelError(string.Empty, "Invalid Login!");
            return this.View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
