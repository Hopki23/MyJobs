namespace MyJobs.Core.Services
{
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Profile;
    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;

    public class ProfileService : IProfileService
    {
        private readonly IDbRepository repository;

        public ProfileService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public UserProfileViewModel GetUserById(string id, string role)
        {
            var userProfile = new UserProfileViewModel();

            if (role == RoleConstants.Employee)
            {
                var employee = this.repository
                    .AllReadonly<Employee>()
                    .Include(e => e.User)
                    .Where(e => e.UserId == id.ToString())
                    .FirstOrDefault();
                userProfile.FirstName = employee!.FirstName;
                userProfile.LastName = employee!.LastName;
                userProfile.Email = employee.User.Email;
                userProfile.IsEmployee = true;
            }
            else if (role == RoleConstants.Employer)
            {
                var employer = this.repository.AllReadonly<Employer>()
                    .Include(e => e.Company)
                    .Include(e => e.User)
                    .Where(e => e.UserId == id.ToString())
                    .FirstOrDefault();
                if (employer == null)
                {
                    throw new InvalidOperationException();
                }

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
    }
}
