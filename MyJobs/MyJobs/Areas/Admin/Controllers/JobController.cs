namespace MyJobs.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Services.Contracts;
    public class JobController : BaseController
    {
        private readonly IJobService jobService;

        public JobController(IJobService jobService)
        {
            this.jobService = jobService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await this.jobService.GetAllJobs();
            return View(model);
        }
    }
}
