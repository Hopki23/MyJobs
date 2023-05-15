namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MyJobs.Infrastructure.Data.Models;
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(CreateCategories());
        }

        private List<Category> CreateCategories()
        {
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                   Name = "Software Engineer"
                },
                new Category()
                {
                    Id = 2,
                   Name = "Food and Hospitality"
                },
                new Category()
                {
                    Id = 3,
                   Name = "Aviation and Aerospace"
                },
                new Category()
                {
                    Id = 4,
                   Name = "Real Estate"
                },
                new Category()
                {
                    Id = 5,
                   Name = "Education and Training"
                },
                new Category()
                {
                    Id = 6,
                   Name = "Marketing and Advertising"
                },
                new Category()
                {
                    Id = 7,
                   Name = "Healthcare and Medical"
                },
                new Category()
                {
                    Id = 8,
                   Name = "Full-Stack Developer"
                },
                new Category()
                {
                    Id = 9,
                   Name = "Back-End Developer"
                },
                new Category()
                {
                    Id = 10,
                   Name = "Front-End Developer"
                },
                new Category()
                {
                    Id = 11,
                   Name = "QA Tester"
                },
            };

            return categories;
        }
    }
}
