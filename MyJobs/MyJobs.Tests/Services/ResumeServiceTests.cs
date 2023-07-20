namespace MyJobs.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    
    using Moq;
    using NUnit.Framework;
    
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Core.Services;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    [TestFixture]
    public class ResumeServiceTests
    {
        private MyJobsDbContext context;
        private IDbRepository repository;
        private IResumeService resumeService;

        [SetUp]
        public void SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<MyJobsDbContext>()
                .UseInMemoryDatabase("MyJobs")
                .Options;

            context = new MyJobsDbContext(contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public void GenerateResumePdfShouldWorkCorrectly()
        {
            this.repository = new DbRepository(context);
            this.resumeService = new ResumeService(repository);

            var model = new ResumeViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Software Engineer",
                Summary = "damm",
                Education = "damm",
                Experience = "da",
                Address = "da",
                PhoneNumber = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                Skills = "samo c#"
            };

            var pdfBytes = this.resumeService.GenerateResumePDF(model);

            Assert.That(pdfBytes, Is.Not.Null);
            Assert.That(pdfBytes.Length, Is.GreaterThan(0));
        }

        [Test]
        public async Task SaveResumeShouldThrowExceptionForNotExistingEmployee()
        {
            this.repository = new DbRepository(context);
            this.resumeService = new ResumeService(repository);

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = new ResumeViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Software Engineer",
                Summary = "damm",
                Education = "damm",
                Experience = "da",
                Address = "da",
                PhoneNumber = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                Skills = "samo c#"
            };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await this.resumeService.SaveResume(model, 12));
            Assert.That(exception.Message, Is.EqualTo("Invalid employee"));
        }

        [Test]
        public async Task SaveResumeShouldWorkCorrectly()
        {
            this.repository = new DbRepository(context);
            this.resumeService = new ResumeService(repository);

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = new ResumeViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Software Engineer",
                Summary = "damm",
                Education = "damm",
                Experience = "da",
                Address = "da",
                PhoneNumber = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                Skills = "samo c#",
            };

            await this.resumeService.SaveResume(model, employee.Id);

            var cv = await this.repository.AllReadonly<CV>().FirstOrDefaultAsync();

            Assert.That(cv, Is.Not.Null);
            Assert.That(cv.ResumeFileName, Is.EqualTo($"{employee.FirstName}_{employee.LastName}.pdf"));

        }
    }
}
