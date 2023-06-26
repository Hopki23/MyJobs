namespace MyJobs.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Services.Contracts;
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await this.userService.GetAllUsers();
            return View(users);
        }
    }
}
