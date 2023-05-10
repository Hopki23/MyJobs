namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserRolesConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(CreateUserRoles());
        }

        private List<IdentityUserRole<string>> CreateUserRoles()
        {
            var userRoles = new List<IdentityUserRole<string>>();

            var userRole = new IdentityUserRole<string>()
            {
                RoleId = "b344e7f4-4914-410a-9670-00056083dcb6",
                UserId = "ed6f8b16-e6bd-4092-b51e-69137932c8c3",
            };

            userRoles.Add(userRole);

            //userRole = new IdentityUserRole<string>()
            //{
            //    RoleId = "165b785b-77f3-4a97-9b27-5b2639b07e63",
            //    UserId = "d7e1b561-cb65-4fca-940c-9c4ccaa2bf12",
            //};

            //userRoles.Add(userRole);

            //userRole = new IdentityUserRole<string>()
            //{
            //    RoleId = "e46bdf31-afd5-4799-9b0e-f58c9a4f132e",
            //    UserId = "32acaebd-abf6-43b1-b73b-08c2667f96a6",
            //};

            //userRoles.Add(userRole);

            return userRoles;
        }
    }
}
