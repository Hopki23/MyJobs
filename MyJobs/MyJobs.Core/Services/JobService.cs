namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Models;

    public class JobService : IJobService
    {
        private readonly IDbRepository repository;

        public JobService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(CreateJobViewModel model, int employerId, int companyId)
        {
            var job = new Job
            {
                Title = model.Title,
                Town = model.TownName,
                CreatedOn = DateTime.UtcNow,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Requirements = model.Requirements,
                Responsibilities = model.Responsibilities,
                Offering = model.Offering,
                EmployerId = employerId,
                CompanyId = companyId
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<JobsViewModel> GetAllJobs(int page, int itemsToTake)
        {
            var jobs = this.repository.AllReadonly<Job>()
                 .OrderByDescending(j => j.CreatedOn)
                 .Skip((page - 1) * itemsToTake)
                 .Take(itemsToTake)
                 .Select(j => new JobsViewModel
                 {
                     Id = j.Id,
                     Title = j.Title,
                     CategoryName = j.Category.Name,
                     CategoryId = j.CategoryId
                 })
                 .ToList();

            return jobs;
        }

        public SingleJobViewModel GetSingleJob(int id)
        {
            return this.repository.AllReadonly<Job>()
                .Where(j => j.Id == id)
                .Select(j => new SingleJobViewModel
                {
                    Title = j.Title,
                    Town = j.Town,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Offering = j.Offering,
                    CreatedOn = j.CreatedOn,
                    CompanyName = j.Company.CompanyName,
                    PhoneNumber = j.Company.PhoneNumber,
                    Address = j.Company.Address,
                    EmployerFirstName = j.Employer.FirstName,
                    EmployerLastName = j.Employer.LastName
                })
                .FirstOrDefault()!;
        }

        public int GetTotalJobCount()
        {
            return this.repository.AllReadonly<Job>().Count();
        }
    }
}
