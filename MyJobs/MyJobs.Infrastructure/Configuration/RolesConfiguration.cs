namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(CreateRoles());
        }

        private List<IdentityRole> CreateRoles()
        {
            var roles = new List<IdentityRole>();

            var role = new IdentityRole()
            {
                Id = "b344e7f4-4914-410a-9670-00056083dcb6",
                ConcurrencyStamp = "b344e7f4-4914-410a-9670-00056083dcb6",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            };

            roles.Add(role);

            //role = new IdentityRole()
            //{
            //    Id = "165b785b-77f3-4a97-9b27-5b2639b07e63",
            //    ConcurrencyStamp = "165b785b-77f3-4a97-9b27-5b2639b07e63",
            //    Name = "Employer",
            //    NormalizedName = "EMPLOYER"
            //};

            //roles.Add(role);

            //role = new IdentityRole()
            //{
            //    Id = "e46bdf31-afd5-4799-9b0e-f58c9a4f132e",
            //    ConcurrencyStamp = "e46bdf31-afd5-4799-9b0e-f58c9a4f132e",
            //    Name = "Employee",
            //    NormalizedName = "EMPLOYEE"
            //};

            //roles.Add(role);

            return roles;
        }
    }
}
