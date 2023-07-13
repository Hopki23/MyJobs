namespace MyJobs.Tests.Services
{
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Models;

    [TestFixture]
    public class EmploymentServiceTests
    {
        private MyJobsDbContext context;
        private IDbRepository repository;
        private IEmploymentService employmentService;

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

        //Approve method

        [Test]
        public async Task ApproveShouldThrowExceptionForNotExistingJobTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

            var invalidJobId = 100;
            var employeeId = 1;
            var employerId = 1;
            var companyId = 1;

            var job = new Job()
            {
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await employmentService.Approve(employeeId, employerId, companyId, invalidJobId));

            Assert.That(exception.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task ApproveShouldWorkCorrectlyTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

            var job = new Job
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

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var company = new Company
            {
                Id = 1,
                Address = "",
                PhoneNumber = "",
                CompanyName = ""
            };

            var resume = new CV
            {
                EmployeeId = 1,
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = new byte[0],
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = ""
            };

            job.Resumes.Add(resume);

            await repository.AddAsync(job);
            await repository.AddAsync(employee);
            await repository.AddAsync(employer);
            await repository.AddAsync(company);
            await repository.SaveChangesAsync();

            await employmentService.Approve(1, 1, 1, 1);

            // Check if the job's resumes collection doesn't contain the removed resume
            var updatedJob = await repository.GetByIdAsync<Job>(1);
            Assert.Multiple(() =>
            {
                Assert.That(updatedJob.Resumes, Does.Not.Contain(resume));

                // Check if the employee is added to the job's employees collection
                Assert.That(updatedJob.Employees, Contains.Item(employee));

                // Check if the employee has the employment record
                Assert.That(employee.Employments, Has.One.Items);

                // Check if the employer has the notification
                Assert.That(employer.Notifications, Has.One.Items);

                // Check if the employee has the notification
                Assert.That(employee.Notifications, Has.One.Items);
            });
        }

        [Test]
        public async Task ApproveShouldThrowExceptionForNotExistingCVTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

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
                ResumeFile = new byte[0],
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = ""
            };

            job.Resumes.Add(cv);

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        await this.employmentService.Approve(5, 5, 5, 1));

            Assert.That(exception.Message, Is.EqualTo("The requested resume was not found."));
        }

        [Test]
        public async Task ApproveShouldThrowExceptionForAlreadyApprovedUser()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

            var job = new Job
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

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var company = new Company
            {
                Id = 1,
                Address = "",
                PhoneNumber = "",
                CompanyName = ""
            };

            var resume = new CV
            {
                EmployeeId = 1,
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = new byte[0],
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = ""
            };

            job.Resumes.Add(resume);
            employee.Employments.Add(new EmployeeEmployment { EmployeeId = 1 });

            await repository.AddAsync(job);
            await repository.AddAsync(employee);
            await repository.AddAsync(employer);
            await repository.AddAsync(company);
            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await employmentService.Approve(1, 1, 1, 1);
            });

            Assert.That(exception.Message, Is.EqualTo("The user has already been approved for this job."));
        }

        //Reject method

        [Test]
        public async Task RejectShouldThrowExceptionForNotExistingJobTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

            var invalidJobId = 100;
            var employeeId = 1;
            var employerId = 1;

            var job = new Job()
            {
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "",
                CategoryId = 1,
                IsDeleted = false
            };

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await employmentService.Reject(employeeId, employerId, invalidJobId));

            Assert.That(exception.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task RejectShouldThrowExceptionForNotExistingCVTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

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
                ResumeFile = new byte[0],
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = ""
            };

            job.Resumes.Add(cv);

            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.employmentService.Reject(5, 5, 1));

            Assert.That(exception.Message, Is.EqualTo("The requested resume was not found."));
        }

        [Test]
        public async Task RejectShouldWorkCorrectlyTest()
        {
            repository = new DbRepository(context);
            employmentService = new EmploymentService(repository);

            // Create test data
            var job = new Job
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

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = ""
            };

            var resume = new CV
            {
                EmployeeId = 1,
                Address = "",
                Education = "",
                Experience = "",
                Gender = "",
                PhoneNumber = "",
                ResumeFile = new byte[0],
                ResumeFileName = "",
                Skills = "",
                Summary = "",
                Title = ""

            };

            job.Resumes.Add(resume);

            await repository.AddAsync(job);
            await repository.AddAsync(employee);
            await repository.AddAsync(employer);
            await repository.SaveChangesAsync();

            await employmentService.Reject(1, 1, 1);

            var rejectedJob = await repository.GetByIdAsync<Job>(1);
            var employeeNotifications = employee.Notifications.ToList();
            var employerNotifications = employer.Notifications.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(rejectedJob.Resumes, Is.Empty);
                Assert.That(employeeNotifications, Has.Count.EqualTo(1));
                Assert.That(employerNotifications, Has.Count.EqualTo(1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(employeeNotifications[0].Message, Is.EqualTo("We are sorry, but you are not approved to work with us."));
                Assert.That(employerNotifications[0].Message, Is.EqualTo("We are sorry, but you are not approved to work with us."));
            });
        }
    }
}