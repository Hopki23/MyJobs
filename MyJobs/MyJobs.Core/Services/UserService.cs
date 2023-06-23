namespace MyJobs.Core.Services
{
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.User;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class UserService : IUserService
    {
        private readonly IDbRepository repository;

        public UserService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<UserListViewModel>> GetAllUsers()
        {
            return await this.repository.AllReadonly<ApplicationUser>()
                .Select(u => new UserListViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.UserName,
                })
                .ToListAsync();
        }
    }
}
