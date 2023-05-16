namespace MyJobs.Core.Services
{
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
                Offering = model.Offering,
                EmployerId = employerId,
                CompanyId = companyId
            };

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();
        }
    }
}
