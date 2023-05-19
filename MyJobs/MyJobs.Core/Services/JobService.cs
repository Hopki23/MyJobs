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
                CategoryId = model.CategoryId,
                Description = model.Description,
                Requirements = model.Requirements,
                Responsibilities = model.Responsibilities,
                Offering = model.Offering,
                EmployerId = employerId,
                CompanyId = companyId
            };

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();
        }

        public IEnumerable<JobsViewModel> GetAllJobs(int page, int itemsToTake)
        {
            var jobs = this.repository.AllReadonly<Job>()
                 .OrderByDescending(j => j.Id)
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

        public int GetTotalJobCount()
        {
            return this.repository.AllReadonly<Job>().Count();
        }
    }
}
