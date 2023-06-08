namespace MyJobs.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;

    public class MyJobsDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyJobsDbContext(DbContextOptions<MyJobsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Employer> Employers { get; set; } = null!;
        public DbSet<CV> CVs { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<EmployeeEmployment> EmployeeEmployments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeEmployment>()
                .HasKey(ee => ee.Id);

            modelBuilder.Entity<EmployeeEmployment>()
                .HasOne(c => c.Company)
                .WithMany(e => e.EmployeeEmployments)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeEmployment>()
                .HasOne(ee => ee.Employer)
                .WithMany(er => er.EmployeeEmployments)
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeEmployment>()
                 .HasOne(e => e.Employee)
                 .WithMany(ee => ee.Employments)
                 .HasForeignKey(e => e.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employer>()
               .HasMany(e => e.Jobs)
               .WithOne(j => j.Employer)
               .HasForeignKey(j => j.EmployerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
               .HasOne(j => j.Employer)
               .WithMany(e => e.Jobs)
               .HasForeignKey(j => j.EmployerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
               .Property(j => j.Salary)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                 .HasOne(n => n.Employer)
                 .WithMany(e => e.Notifications)
                 .HasForeignKey(n => n.EmployerId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Employee)
                .WithMany(e => e.Notifications)
                .HasForeignKey(n => n.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}