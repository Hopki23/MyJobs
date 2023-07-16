namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.User;
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetAllUsers();
        Task DisableUserAsync(string id);
        Task EnableUserAsync(string id);
    }
}
