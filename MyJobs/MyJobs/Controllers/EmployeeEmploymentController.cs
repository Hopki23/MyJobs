namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Services;

    public class EmployeeEmploymentController : BaseController
    {
        private readonly IEmploymentService employmentService;

        public EmployeeEmploymentController(
            IEmploymentService employmentService)
        {
            this.employmentService = employmentService;
        }


        [HttpPost]
        public async Task<IActionResult> Approve(int employeeId, int employerId, int companyId, int jobId)
        {
            var isApproved = await this.employmentService.Approve(employeeId, employerId, companyId, jobId);

            if (!isApproved)
            {
                return NotFound();
            }

            return RedirectToAction("All", "Jobs");
        }
    }
}