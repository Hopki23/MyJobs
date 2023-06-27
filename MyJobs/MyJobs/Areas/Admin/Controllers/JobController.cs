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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await this.jobService.GetAllJobs();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await this.jobService.GetById(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await this.jobService.Delete(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}
