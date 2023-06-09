namespace MyJobs.Core.Services
{
    public interface IEmploymentService
    {
        Task<bool> Approve(int employeeId, int employerId, int companyId, int jobId);
    }
}
