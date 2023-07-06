namespace MyJobs.Core.Services
{
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;
    public class EmploymentService : IEmploymentService
    {
        private readonly IDbRepository repository;

        public EmploymentService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task Approve(int employeeId, int employerId, int companyId, int jobId)
        {
            var job = await this.repository.All<Job>()
                    .Include(j => j.Resumes)
                    .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            var cvToRemove = job.Resumes.FirstOrDefault(c => c.EmployeeId == employeeId);

            if (cvToRemove == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            //var existingEmployment = employee.Employments.FirstOrDefault(c => c.EmployeeId == employeeId);

            //if (existingEmployment != null)
            //{
            //    throw new InvalidOperationException("The user has already been approved for this job.");
            //}

            job.Resumes.Remove(cvToRemove);

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

            employee.Jobs.Add(job);
            employer.Notifications.Add(notification);
            employee.Notifications.Add(notification);
            job.Employees.Add(employee);

            await this.repository.AddAsync(employeeEmployment);
            await this.repository.AddAsync(notification);
            await this.repository.SaveChangesAsync();
        }

        public async Task Reject(int employeeId, int employerId, int jobId)
        {
            var job = await this.repository.All<Job>()
                  .Include(j => j.Resumes)
                  .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            var cvToRemove = job.Resumes.FirstOrDefault(c => c.EmployeeId == employeeId);

            if (cvToRemove == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            job.Resumes.Remove(cvToRemove);

            var notification = new Notification()
            {
                EmployeeId = employeeId,
                EmployerId = employerId,
                Message = "We are sorry, but you are not approved to work with us.",
                CreatedAt = DateTime.Now
            };

            var employee = await this.repository.GetByIdAsync<Employee>(employeeId);
            var employer = await this.repository.GetByIdAsync<Employer>(employerId);

            employer.Notifications.Add(notification);
            employee.Notifications.Add(notification);

            await this.repository.AddAsync(notification);
            await this.repository.SaveChangesAsync();
        }
    }
}
