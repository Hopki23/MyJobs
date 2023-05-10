namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MyJobs.Infrastructure.Models;

    public class EmployerConfiguration : IEntityTypeConfiguration<Employer>
    {
        public void Configure(EntityTypeBuilder<Employer> builder)
        {
            builder.HasData(CreateEmployers());
        }

        private List<Employer> CreateEmployers()
        {
            List<Employer> employers = new List<Employer>()
            {
                new Employer()
                {
                    EmployerId = 1,
                    FirstName = "Shef",
                    LastName = "Shecheto",
                    UserId = "d7e1b561-cb65-4fca-940c-9c4ccaa2bf12",
                    CompanyId = 1
                }
            };

            return employers;
        }
    }
}
