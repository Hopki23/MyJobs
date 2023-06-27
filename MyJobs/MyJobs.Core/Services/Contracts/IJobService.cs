namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    public interface IJobService
    {
        Task CreateAsync(CreateJobViewModel model, int employerId, int companyId);

        Task<IEnumerable<JobsViewModel>> GetAllJobs();

        Task<IEnumerable<JobsViewModel>> GetAllJobs(int page, int itemsToTake = 5);

        int GetTotalJobCount();

        Task<SingleJobViewModel> GetSingleJob(int id, Employer employer);

        Task Apply(UploadResumeViewModel model, Employee employee);

        Task<IEnumerable<JobsWithCVsViewModel>> GetJobsWithCV(JobsWithCVsViewModel model, Employer employer);

        Task<EditJobViewModel> GetById(int id, string userId);

        Task GetById(int id);

        Task Update(int id, EditJobViewModel model);

        Task Delete(int id);

        Task<JobFilterViewModel> GetJobFilterViewModel();

        IEnumerable<Job> FilterJobOffers(string select, string[] selectedWorkingTimes, string locationSelect);

        Task<IEnumerable<JobsViewModel>> GetJobsForCertainEmployer(Employer employer);

        Task<IEnumerable<Job>> GetJobsByEmployeeId(int employeeId);
    }
}
