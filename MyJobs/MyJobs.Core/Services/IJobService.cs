﻿namespace MyJobs.Core.Services
{

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    public interface IJobService
    {
        Task CreateAsync(CreateJobViewModel model, int employerId, int companyId);

        IEnumerable<JobsViewModel> GetAllJobs(int page, int itemsToTake = 5);

        int GetTotalJobCount();

        SingleJobViewModel GetSingleJob(int id, Employer employer);

        Task Apply(UploadResumeViewModel model, Employee employee);

        IEnumerable<JobsWithCVsViewModel> GetJobsWithCV(JobsWithCVsViewModel model, Employer employer);

        EditJobViewModel GetById(int id);

        Task Update(int id, EditJobViewModel model);

        Task Delete(int id);
    }
}
