namespace MyJobs.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;

    public class EmployeeEmploymentController : BaseController
    {
        private readonly IDbRepository repository;

        public EmployeeEmploymentController(
            IDbRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int employeeId, int employerId, int companyId)
        {
            var cv = this.repository.AllReadonly<CV>()
                .Where(c => c.EmployeeId == employeeId)
                .FirstOrDefault();

            if (cv != null)
            {
                cv.IsDeleted = true;
            }

            var employeeEmployment = new EmployeeEmployment
            {
                EmployeeId = employeeId,
                EmployerId = employerId,
                CompanyId = companyId
            };

            await this.repository.AddAsync(employeeEmployment);

            var notification = new Notification()
            {
                EmployeeId = employeeId,
                EmployerId = employerId,
                Message = "Congrats, you are approved to work with us!",
                CreatedAt = DateTime.Now
            };

            await this.repository.AddAsync(notification);

            var employee = await this.repository.GetByIdAsync<Employee>(employeeId);
            var employer = await this.repository.GetByIdAsync<Employer>(employerId);

            employee.Notifications.Add(notification);
            employer.Notifications.Add(notification);

            await this.repository.SaveChangesAsync();

            return RedirectToAction("All", "Jobs");
        }
    }
}
