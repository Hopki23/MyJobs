namespace MyJobs.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Profile;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;

    public class ProfileService : IProfileService
    {
        private readonly IDbRepository repository;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileService(
            IDbRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task EditProfile(UserProfileViewModel model, int id, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == RoleConstants.Employee)
            {
                var employee = await this.repository.All<Employee>()
                    .Where(x => x.UserId == userId)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    throw new ArgumentException("Invalid employee");
                }

                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.User.Email = model.Email;
            }
            else if (role == RoleConstants.Employer)
            {
                var employer = await this.repository.All<Employer>()
                     .Where(x => x.UserId == userId)
                     .Include(x => x.User)
                     .Include(x => x.Company)
                     .FirstOrDefaultAsync();

                if (employer == null )
                {
                    throw new ArgumentException("Invalid employee");
                }

                employer.FirstName = model.FirstName;
                employer.LastName = model.LastName;
                employer.User.Email = model.Email;
                employer.Company.Address = model.CompanyAddress!;
                employer.Company.CompanyName = model.CompanyName!;
                employer.Company.PhoneNumber = model.CompanyPhoneNumber!;
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<UserProfileViewModel> GetProfileForEditing(int id, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == null)
            {
                throw new ArgumentException("Invalid role!");
            }

            UserProfileViewModel model;

            if (role == RoleConstants.Employee)
            {
                var employee = await this.repository.AllReadonly<Employee>()
                    .Where(x => x.Id == id)
                    .Include(e => e.User)
                    .FirstOrDefaultAsync();

                if (employee == null || employee.UserId != userId)
                {
                    throw new ArgumentException("Invalid employee!");
                }

                model = new UserProfileViewModel
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.User.Email,
                    IsEmployee = true
                };
            }
            else if (role == RoleConstants.Employer)
            {
                var employer = await this.repository.AllReadonly<Employer>()
                    .Where(x => x.Id == id)
                    .Include(x => x.User)
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync();

                if (employer == null || employer.UserId != userId)
                {
                    throw new ArgumentException("Invalid employer!");
                }

                model = new UserProfileViewModel
                {
                    FirstName = employer.FirstName,
                    LastName = employer.LastName,
                    Email = employer.User.Email,
                    CompanyAddress = employer.Company.Address,
                    CompanyName = employer.Company.CompanyName,
                    CompanyPhoneNumber = employer.Company.PhoneNumber,
                    IsEmployee = false
                };
            }
            else
            {
                throw new ArgumentException("Invalid role!");
            }

            return model;
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsForEmployee(int employeeId)
        {
            return await this.repository.AllReadonly<Notification>()
           .Include(n => n.Employer)
           .Include(n => n.Employer.Company)
           .Where(n => n.EmployeeId == employeeId && !n.IsRead)
           .ToListAsync();
        }

        public async Task<UserProfileViewModel> GetUserById(string id, string role)
        {
            var userProfile = new UserProfileViewModel();

            if (role == RoleConstants.Employee)
            {
                var employee = await this.repository
                    .AllReadonly<Employee>()
                    .Include(e => e.User)
                    .Where(e => e.UserId == id.ToString())
                    .FirstOrDefaultAsync();

                userProfile.Id = employee!.Id;
                userProfile.FirstName = employee!.FirstName;
                userProfile.LastName = employee!.LastName;
                userProfile.Email = employee.User.Email;
                userProfile.IsEmployee = true;
            }
            else if (role == RoleConstants.Employer)
            {
                var employer = await this.repository.AllReadonly<Employer>()
                    .Include(e => e.Company)
                    .Include(e => e.User)
                    .Where(e => e.UserId == id.ToString())
                    .FirstOrDefaultAsync();
                if (employer == null)
                {
                    throw new InvalidOperationException();
                }

                userProfile.Id = employer!.Id;
                userProfile.FirstName = employer.FirstName;
                userProfile.LastName = employer.LastName;
                userProfile.Email = employer.User.Email;
                userProfile.CompanyName = employer.Company.CompanyName;
                userProfile.CompanyAddress = employer.Company.Address;
                userProfile.CompanyPhoneNumber = employer.Company.PhoneNumber;
            }
            else
            {
                throw new InvalidOperationException(ErrorConstants.InvalidUserRole);
            }

            return userProfile;
        }

        public async Task<Notification> MarkNotificationAsRead(int notificationId)
        {
            var notification = await this.repository.GetByIdAsync<Notification>(notificationId);

            notification.IsRead = true;
            await this.repository.SaveChangesAsync();

            return notification;
        }
    }
}
