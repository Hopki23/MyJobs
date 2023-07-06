namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;

    public class EmployeeEmploymentController : BaseController
    {
        private readonly IEmploymentService employmentService;

        public EmployeeEmploymentController(
            IEmploymentService employmentService)
        {
            this.employmentService = employmentService;
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Approve(int employeeId, int employerId, int companyId, int jobId)
        {
            try
            {
                await this.employmentService.Approve(employeeId, employerId, companyId, jobId);

                return RedirectToAction("ReceivedResumes", "Jobs");
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employer)]
        public async Task<IActionResult> Reject(int employeeId, int employerId, int jobId)
        {
            try
            {
                await this.employmentService.Reject(employeeId, employerId, jobId);

                return RedirectToAction("ReceivedResumes", "Jobs");
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}