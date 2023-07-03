namespace MyJobs.Core.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.User;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class UserService : IUserService
    {
        private readonly IDbRepository repository;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(
            IDbRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task DisableUserAsync(string id)
        {
            var user = await this.repository.GetByIdAsync<ApplicationUser>(id);

            user.IsDeleted = true;

            await this.repository.SaveChangesAsync();
        }

        public async Task EnableUserAsync(string id)
        {
            var user = await this.repository.GetByIdAsync<ApplicationUser>(id);

            user.IsDeleted = false;

            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserListViewModel>> GetAllUsers()
        {
            var users =  await this.repository.All<ApplicationUser>()
                .Select(u => new UserListViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.UserName,
                    IsDisabled = u.IsDeleted
                })
                .ToListAsync();

            foreach (var user in users)
            {
                var appUser = await this.userManager.FindByIdAsync(user.Id);
                var role = await this.userManager.GetRolesAsync(appUser);

                user.RoleName = role.FirstOrDefault()!;
            }

            return users;
        }
    }
}
