namespace MyJobs.Tests.Services
{
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Models;

    [TestFixture]
    public class JobServiceTests
    {
        private MyJobsDbContext context;
        private IDbRepository repository;
        private IJobService jobService;
        private ICategoryService categoryService;


        [SetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<MyJobsDbContext>()
                .UseInMemoryDatabase("MyJobs")
                .Options;

            context = new MyJobsDbContext(contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public async Task ApplyShouldThrowExceptionForNotExistingResumeTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 1
            };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await jobService.Apply(model, userId));

            Assert.That(exception.Message, Is.EqualTo(NotificationConstants.CreateResumeError));
        }

        [Test]
        public async Task ApplyShouldThrowExceptionForNotExistingJobTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            var cv = new CV
            {
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = Array.Empty<byte>(),
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = "",
                EmployeeId = employee.Id
            };

            await repository.AddAsync(employee);
            await repository.AddAsync(cv);
            await repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 2 // Non-existent job ID
            };

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await jobService.Apply(model, userId));

            Assert.That(exception.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task ApplyShouldThrowExceptionWhenJobIsMarkedAsDeletedTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var cv = new CV
            {
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = Array.Empty<byte>(),
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = "",
                EmployeeId = employee.Id
            };

            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = true
            };

            await this.repository.AddAsync(employee);
            await this.repository.AddAsync(cv);
            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 1
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Apply(model, userId));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task ApplyShouldThrowExceptionWhenUserTriesToApplyIfHeAlreadyHadTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            var cv = new CV
            {
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = Array.Empty<byte>(),
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = "",
                EmployeeId = employee.Id
            };

            job.Resumes.Add(cv);
            
            await repository.AddAsync(employee);
            await repository.AddAsync(cv);
            await repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 1 
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await this.jobService.Apply(model, userId));
            Assert.That(ex.Message, Is.EqualTo(NotificationConstants.AlreadyAppliedMessageError));
        }

        [Test]
        public async Task ApplyShouldThrowExceptionWhenUserTriesToApplyIfHeAlreadyIsApprovedTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            var cv = new CV
            {
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = Array.Empty<byte>(),
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = "",
                EmployeeId = employee.Id
            };

            job.Employees.Add(employee);

            await repository.AddAsync(employee);
            await repository.AddAsync(cv);
            await repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 1
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Apply(model, userId));
            Assert.That(ex.Message, Is.EqualTo(NotificationConstants.AlreadyApprovedMessageError));
        }

        [Test]
        public async Task ApproveShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "dasjk";

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            var cv = new CV
            {
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = Array.Empty<byte>(),
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = "",
                EmployeeId = employee.Id
            };


            await repository.AddAsync(employee);
            await repository.AddAsync(cv);
            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var model = new UploadResumeViewModel
            {
                Id = 1
            };

            await this.jobService.Apply(model, userId);
            Assert.Multiple(() =>
            {
                Assert.That(cv.Jobs, Has.Count.EqualTo(1));
                Assert.That(cv.Jobs, Is.Not.Null);

                Assert.That(job.Resumes, Has.Count.EqualTo(1));
                Assert.That(job.Resumes, Is.Not.Null);
            });
        }
    }
}