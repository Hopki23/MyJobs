namespace MyJobs.Infrastructure.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using MyJobs.Infrastructure.Models;

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasData(CreateEmployee());
        }

        private List<Employee> CreateEmployee()
        {
            List<Employee> employees = new List<Employee>()
            {
                new Employee()
                {
                    EmployeeId = 1,
                    FirstName = "Gosho",
                    LastName = "Petrov",
                    EmployerId = 1,
                    Gender = "Male",
                    Address = "Ne znam",
                    UserId = "32acaebd-abf6-43b1-b73b-08c2667f96a6",
                    CompanyId = 1
                }
            };

            return employees;
        }
    }
}
