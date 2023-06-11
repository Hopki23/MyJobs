namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Resume;
    public interface IResumeService
    {
        Task SaveResume(ResumeViewModel model, int employeeId);
        byte[] GenerateResumePDF(ResumeViewModel model);
    }
}
