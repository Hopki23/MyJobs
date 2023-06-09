namespace MyJobs.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    
    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;
    public class EmploymentService : IEmploymentService
    {
        private readonly IDbRepository repository;

        public EmploymentService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Approve(int employeeId, int employerId, int companyId, int jobId)
        {
            var cv = await this.repository.AllReadonly<CV>()
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId);

            if (cv == null)
            {
                return false;
            }

            var employeeEmployment = new EmployeeEmployment
            {
                EmployeeId = employeeId,
                EmployerId = employerId,
                CompanyId = companyId
            };

            var notification = new Notification()
            {
                EmployeeId = employeeId,
                EmployerId = employerId,
                Message = "Congrats, you are approved to work with us!",
                CreatedAt = DateTime.Now
            };

            var employee = await this.repository.GetByIdAsync<Employee>(employeeId);
            var employer = await this.repository.GetByIdAsync<Employer>(employerId);
            var job = await this.repository.GetByIdAsync<Job>(jobId);

            employee.Jobs.Add(job);
            employer.Notifications.Add(notification);
            employee.Notifications.Add(notification);
            job.Employees.Add(employee);


            await this.repository.AddAsync(employeeEmployment);
            await this.repository.AddAsync(notification);
            await this.repository.SaveChangesAsync();

            return true;
        }
    }
}
