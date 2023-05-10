namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MyJobs.Infrastructure.Models;

    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasData(CreateCompany());
        }

        private List<Company> CreateCompany()
        {
            List<Company> companies = new List<Company>()
            {
                new Company()
                {
                    Id = 1,
                    CompanyName = "Asparuhovo",
                    Address = "Asparuhovoo",
                    PhoneNumber = "12345678"
                }
            };

            return companies;
        }
    }
}
