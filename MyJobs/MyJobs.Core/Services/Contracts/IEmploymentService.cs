namespace MyJobs.Core.Services.Contracts
{
    public interface IEmploymentService
    {
        Task<bool> Approve(int employeeId, int employerId, int companyId, int jobId);
        Task<bool> Reject(int employeeId, int employerId, int jobId);
    }
}
