namespace MyJobs.Infrastructure.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Infrastructure.Configuration;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                    .HasOne(e => e.Employer)
                    .WithMany(e => e.Employees)
                    .HasForeignKey(e => e.EmployerId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                    .HasOne(c => c.Company)
                    .WithMany(c => c.Employees)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.Employees)
                .WithOne(e => e.Employer)
                .HasForeignKey(e => e.EmployerId)
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
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RolesConfiguration());
            modelBuilder.ApplyConfiguration(new UserRolesConfiguration());
            //modelBuilder.ApplyConfiguration(new JobConfiguration());
            //modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            //modelBuilder.ApplyConfiguration(new EmployerConfiguration());
            //modelBuilder.ApplyConfiguration(new CompanyConfiguration());

        }
    }
}