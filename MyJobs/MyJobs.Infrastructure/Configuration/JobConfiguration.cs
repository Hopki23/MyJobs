namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MyJobs.Infrastructure.Models;
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasData(CreateJob());
        }

        private List<Job> CreateJob()
        {
            List<Job> jobs = new List<Job>()
            {
                new Job()
                {
                   Id = 1,
                   Title = "Nai qkata rabota",
                   Description = "Nai qkata rabotaaaa",
                   Requirements = "Ne znamm",
                   Offering = "nishto",
                   CompanyId = 1,
                   EmployerId = 1
                }
            };

            return jobs;
         }
    }
}
