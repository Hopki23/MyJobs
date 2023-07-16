namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    public interface IJobService
    {
        Task CreateAsync(CreateJobViewModel model, string userId);

        Task<IEnumerable<JobsViewModel>> GetAllJobs();

        Task<IEnumerable<JobsViewModel>> GetAllJobs(int page, int itemsToTake = 5);

        Task<int> GetTotalJobCount();

        Task<SingleJobViewModel> GetSingleJob(int id, string userId);

        Task Apply(UploadResumeViewModel model, string userId);

        Task<IEnumerable<JobsWithCVsViewModel>> GetJobsWithCV(JobsWithCVsViewModel model, string userId);

        Task<EditJobViewModel> GetById(int id, string userId);

        Task ApproveJob(int id);

        Task Update(int id, EditJobViewModel model);

        Task Delete(int id);

        Task<JobFilterViewModel> GetJobFilterViewModel();

        IEnumerable<Job> FilterJobOffers(string select, string[] selectedWorkingTimes, string locationSelect);

        Task<IEnumerable<JobsViewModel>> GetJobsForCertainEmployer(string userId);

        Task<IEnumerable<Job>> GetJobsByEmployeeId(string userId);
    }
}
