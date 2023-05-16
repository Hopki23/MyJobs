namespace MyJobs.Core.Services
{
    using MyJobs.Core.Models.Job;
    public interface IJobService
    {
        Task CreateAsync(CreateJobViewModel model, int employerId, int companyId);
    }
}
