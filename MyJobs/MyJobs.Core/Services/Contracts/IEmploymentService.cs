namespace MyJobs.Core.Services.Contracts
{
    public interface IEmploymentService
    {
        Task Approve(int employeeId, int employerId, int companyId, int jobId);
        Task Reject(int employeeId, int employerId, int jobId);
    }
}
