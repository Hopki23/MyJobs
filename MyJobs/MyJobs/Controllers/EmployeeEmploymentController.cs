namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Services.Contracts;

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
            try
            {
                var isApproved = await this.employmentService.Approve(employeeId, employerId, companyId, jobId);
                if (!isApproved)
                {
                    return View("CustomError");
                }

                return RedirectToAction("ReceivedResumes", "Jobs");
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int employeeId, int employerId, int jobId)
        {
            try
            {
                var isApproved = await this.employmentService.Reject(employeeId, employerId, jobId);
                if (!isApproved)
                {
                    return View("CustomError");
                }

                return RedirectToAction("ReceivedResumes", "Jobs");
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}