namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasData(CreateUsers());
        }

        private List<ApplicationUser> CreateUsers()
        {
            var users = new List<ApplicationUser>();
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser()
            {
                Id = "ed6f8b16-e6bd-4092-b51e-69137932c8c3",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@abv.bg",
                NormalizedEmail = "ADMIN@ABV.BG",
                FirstName = "Admin",
                LastName = "Adminov",
                PasswordHash = passwordHasher.HashPassword(null, "admin123"),
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            users.Add(user);

            //user = new ApplicationUser()
            //{
            //    Id = "d7e1b561-cb65-4fca-940c-9c4ccaa2bf12",
            //    UserName = "employer",
            //    NormalizedUserName = "EMPLOYER",
            //    Email = "employer@abv.bg",
            //    NormalizedEmail = "EMPLOYER@ABV.BG",
            //    FirstName = "Employer",
            //    LastName = "Employerov",
            //    PasswordHash = passwordHasher.HashPassword(null, "employer123"),
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //};

            //users.Add(user);

            //user = new ApplicationUser()
            //{
            //    Id = "32acaebd-abf6-43b1-b73b-08c2667f96a6",
            //    UserName = "employee",
            //    NormalizedUserName = "EMPLOYEE",
            //    Email = "employee@abv.bg",
            //    NormalizedEmail = "EMPLOYEE@ABV>BG",
            //    FirstName = "Employee",
            //    LastName = "Employeerov",
            //    PasswordHash = passwordHasher.HashPassword(null, "employee123"),
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //};

            //users.Add(user);

            return users;
        }
    }
}
