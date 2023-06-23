namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.User;
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetAllUsers();
    }
}
