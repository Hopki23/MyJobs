namespace MyJobs.Tests.Services
{
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Data.Models;
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

        //[Test]
        //public async Task ApplyShouldThrowExceptionForNotExistingResumeTest()
        //{
        //    var categoryServiceMock = new Mock<ICategoryService>();
        //    var categoryService = categoryServiceMock.Object;

        //    this.repository = new DbRepository(this.context);
        //    this.jobService = new JobService(this.repository, categoryService);

        //    var userId = "dasjk";

        //    var employee = new Employee
        //    {
        //        Id = 1,
        //        FirstName = "",
        //        LastName = "",
        //        UserId = userId
        //    };

        //    await this.repository.AddAsync(employee);
        //    await this.repository.SaveChangesAsync();

        //    var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        //    await jobService.Apply(userId));

        //    Assert.That(exception.Message, Is.EqualTo(NotificationConstants.CreateResumeError));
        //}

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
                Id = 1,
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


            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await jobService.Apply(cv.Id, userId, 27));

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
                Id=1,
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


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Apply(cv.Id,userId,job.Id));
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
                Id=1,
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

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await this.jobService.Apply(cv.Id,userId,job.Id));
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
                Id=1,
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


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Apply(cv.Id,userId,job.Id));
            Assert.That(ex.Message, Is.EqualTo(NotificationConstants.AlreadyApprovedMessageError));
        }

        [Test]
        public async Task ApplyShouldWorkCorrectlyTest()
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
                Id=1,
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


            await this.jobService.Apply(cv.Id,userId,job.Id);
            Assert.Multiple(() =>
            {
                Assert.That(cv.Jobs, Has.Count.EqualTo(1));
                Assert.That(cv.Jobs, Is.Not.Null);

                Assert.That(job.Resumes, Has.Count.EqualTo(1));
                Assert.That(job.Resumes, Is.Not.Null);
            });
        }

        [Test]
        public async Task CreateShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "testUser";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var model = new CreateJobViewModel
            {
                Title = "Job Title",
                TownName = "Job Town",
                CategoryId = 1,
                Description = "Job Description",
                Requirements = "Job Requirements",
                Responsibilities = "Job Responsibilities",
                Offering = "Job Offering",
                Salary = 1000,
                WorkingTime = "Full-Time",
                EmployerId = employer.Id
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            await this.jobService.CreateAsync(model, userId);

            var createdJob = await this.repository.GetByIdAsync<Job>(1);

            Assert.That(createdJob, Is.Not.Null);
        }

        [Test]
        public async Task DeleteShouldThrowExceptionWhenJobDoesNotExistTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Delete(1));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task DeleteShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

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

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            await this.jobService.Delete(1);

            var dbJob = await repository.GetByIdAsync<Job>(1);

            Assert.That(dbJob.IsDeleted, Is.EqualTo(true));
        }

        [Test]
        public async Task FilterJobsShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            await repository.AddRangeAsync(new List<Category>()
            {
                new Category() { Id = 1, Name = "Test", IsDeleted = false },
                new Category() { Id = 2, Name = "Test 2", IsDeleted = false },
                new Category() { Id = 3, Name = "Test 3", IsDeleted = false },
            });

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    CategoryId = 1,
                    IsDeleted = false,
                    WorkingTime = "Remote",
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    CategoryId = 2,
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                },
                new Job()
                {
                    Id = 3,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    CategoryId = 3,
                    IsDeleted = false,
                    WorkingTime = "Full-Time",
                },
            });

            await this.repository.SaveChangesAsync();

            var filteredJobs1 = this.jobService.FilterJobOffers("1", null, null); // Filter by category
            var filteredJobs2 = this.jobService.FilterJobOffers(null, new[] { "Full-Time", "Part-Time" }, null); // by working time
            var filteredJobs3 = this.jobService.FilterJobOffers(null, null, "Town 1"); // Filter by location


            Assert.Multiple(() =>
            {
                Assert.That(filteredJobs1.Count, Is.EqualTo(1));
                Assert.That(filteredJobs2.Count, Is.EqualTo(2));
                Assert.That(filteredJobs3.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetAllJobsShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    CategoryId = 1,
                    IsDeleted = false,
                    WorkingTime = "Remote",
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    CategoryId = 2,
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                },
                new Job()
                {
                    Id = 3,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    CategoryId = 3,
                    IsDeleted = false,
                    WorkingTime = "Full-Time",
                },
                new Job()
                {
                    Id = 4,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    CategoryId = 3,
                    IsDeleted = false,
                    WorkingTime = "Full-Time",
                },
                new Job()
                {
                    Id = 5,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    CategoryId = 3,
                    IsDeleted = false,
                    WorkingTime = "Full-Time",
                },
            });

            await this.repository.SaveChangesAsync();

            await this.jobService.GetAllJobs();

            var jobs = await this.repository.AllReadonly<Job>().ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(jobs, Has.Count.EqualTo(5));
                Assert.That(jobs[0].Id, Is.EqualTo(1));
                Assert.That(jobs[0].Town, Is.EqualTo("Town 1"));
                Assert.That(jobs[jobs.Count - 1].Id, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task GetByIdShouldThrowExceptionWithInvalidJobIdTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "testUser";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    CategoryId = 1,
                    IsDeleted = false,
                    WorkingTime = "Remote",
                    EmployerId = 1,
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    CategoryId = 2,
                    IsDeleted = false,
                    WorkingTime = "Part-Time"
                }
            });

            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.GetById(4, userId));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task GetByIdShouldThrowExceptionWithInvalidUserIdTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "invalid";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = "dassa"
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    CategoryId = 1,
                    IsDeleted = false,
                    WorkingTime = "Remote",
                    EmployerId = 1,
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    CategoryId = 2,
                    IsDeleted = false,
                    WorkingTime = "Part-Time"
                }
            });

            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.GetById(1, userId));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task GetByIdShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            int jobId = 1;
            var userId = "invalid";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId,
            };

            var job = new Job()
            {
                Id = jobId,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "Town 1",
                Title = "",
                CategoryId = 1,
                IsDeleted = false,
                WorkingTime = "Remote",
                EmployerId = 1,
            };

            await this.repository.AddAsync(employer);
            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var result = await this.jobService.GetById(jobId, userId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Title, Is.EqualTo(job.Title));
                Assert.That(result.WorkingTime, Is.EqualTo(job.WorkingTime));
            });
        }

        [Test]
        public async Task ApproveJobShouldThrowExceptionForNotExistingJobTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            int jobId = 1;

            var job = new Job()
            {
                Id = jobId,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "Town 1",
                Title = "",
                CategoryId = 1,
                IsDeleted = false,
                WorkingTime = "Remote",
                EmployerId = 1,
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.ApproveJob(2));
            Assert.That(ex.Message, Is.EqualTo("Job not found!"));
        }

        [Test]
        public async Task ApproveJobShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            int jobId = 1;

            var job = new Job()
            {
                Id = jobId,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "Town 1",
                Title = "",
                CategoryId = 1,
                IsDeleted = false,
                WorkingTime = "Remote",
                EmployerId = 1,
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            await this.jobService.ApproveJob(jobId);

            var dbJob = await this.repository.GetByIdAsync<Job>(jobId);

            Assert.That(dbJob, Is.Not.Null);
            Assert.That(dbJob.IsApproved, Is.True);
        }

        [Test]
        public async Task GetJobFilterViewModelShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            categoryServiceMock.Setup(c => c.GetAllCategories())
                .ReturnsAsync(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("1", "Category 1"),
                    new KeyValuePair<string, string>("2", "Category 2"),
                    new KeyValuePair<string, string>("3", "Category 3")
                });
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            await this.repository.AddRangeAsync(new List<Job>
            {
                new Job { Id = 1, WorkingTime = "Full-Time", Town = "Town 1", Description = "", Offering = "", Requirements = "", Responsibilities = "", Title = "" },
                new Job { Id = 2, WorkingTime = "Part-Time", Town = "Town 2", Description = "", Offering = "", Requirements = "", Responsibilities = "", Title = ""},
                new Job { Id = 3, WorkingTime = "Remote", Town = "Town 3", Description = "", Offering = "", Requirements = "", Responsibilities = "", Title = "" },
                new Job { Id = 4, WorkingTime = "Full-Time", Town = "Town 2", Description = "", Offering = "", Requirements = "", Responsibilities = "", Title = "" }
            });
            await this.repository.SaveChangesAsync();

            var jobFilterViewModel = await this.jobService.GetJobFilterViewModel();

            Assert.Multiple(() =>
            {
                Assert.That(jobFilterViewModel.Categories, Has.Count.EqualTo(3));
                Assert.That(jobFilterViewModel.WorkingTimes, Has.Count.EqualTo(3));
                Assert.That(jobFilterViewModel.TownNames, Has.Count.EqualTo(3));
                Assert.That(jobFilterViewModel.WorkingTimes, Contains.Item("Full-Time"));
                Assert.That(jobFilterViewModel.TownNames, Contains.Item("Town 1"));
            });
        }
        [Test]
        public async Task GetJobsByEmployeeIdShouldWorkCorrectly()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var category = new Category()
            {
                Id = 1,
                Name = "categoryy"
            };

            var company = new Company()
            {
                Id = 1,
                CompanyName = "dsadsa",
                Address = "",
                PhoneNumber = "fas"
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test",
                UserId = "test",
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
                CategoryId = category.Id,
                CompanyId = company.Id,
                IsDeleted = false
            };

            var cv = new CV
            {
                Id = 1,
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

            await repository.AddAsync(category);
            await repository.AddAsync(company);
            await repository.AddAsync(employee);
            await repository.AddAsync(job);
            await repository.AddAsync(cv);
            await repository.SaveChangesAsync();

            var jobs = await this.jobService.GetJobsByEmployeeId(employee.UserId);

            Assert.That(jobs, Is.Not.Null);
            Assert.That(jobs.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetJobsForCertainEmployerShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var employer = new Employer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test",
                UserId = "test",
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    IsDeleted = false,
                    Category = new Category { Id = 1, Name = "Category 3" },
                    WorkingTime = "Remote",
                    EmployerId = 1,
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    IsDeleted = false,
                    Category = new Category { Id = 2, Name = "Category 2" },
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                },
                 new Job()
                {
                    Id = 3,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    IsDeleted = false,
                    Category = new Category { Id = 3, Name = "Category 1" },
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                }
            });

            await repository.AddAsync(employer);
            await repository.SaveChangesAsync();

            var dbJobs = await this.jobService.GetJobsForCertainEmployer("test");

            Assert.Multiple(() =>
            {
                Assert.That(dbJobs, Is.Not.Null);
                Assert.That(dbJobs.Count(), Is.EqualTo(3));
                Assert.That(dbJobs.FirstOrDefault().Id, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task GetSingleJobShouldThrowExceptionForNotExistingJobTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            string userId = "test";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test",
                UserId = userId,
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Remote",
                    EmployerId = 1,
                },
                new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                },
                 new Job()
                {
                    Id = 3,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                }
            });

            await repository.AddAsync(employer);
            await repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.GetSingleJob(54, userId));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task GetSingleJobShouldWorkCorrectlyTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            string userId = "test";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test",
                UserId = userId,
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Remote",
                    Category = new Category { Id = 1, Name = "Category 1" },
                    Company = new Company {Id = 2, CompanyName = "doggg", PhoneNumber = "dgas", Address = "dsag"},
                    EmployerId = 1,
                },
                 new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                    Category = new Category { Id = 2, Name = "Category 1" },
                    Company = new Company {Id = 1, CompanyName = "dog", PhoneNumber = "das", Address = "dsa"},
                    EmployerId= 1,
                }
            });

            await repository.AddAsync(employer);
            await repository.SaveChangesAsync();

            var job = await this.jobService.GetSingleJob(1, userId);

            Assert.That(job, Is.Not.Null);
            Assert.That(job.IsOwner, Is.True);
        }

        [Test]
        public async Task GetTotalJobCountTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Remote",
                    EmployerId = 1,
                    IsApproved = true
                },
                 new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    IsDeleted = false,
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                    IsApproved = true
                }
            });

            await repository.SaveChangesAsync();

            var jobs = await this.jobService.GetTotalJobCount();

            Assert.That(jobs, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateShoulThrowException()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);


            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "Town 1",
                Title = "",
                IsDeleted = false,
                WorkingTime = "Remote",
                EmployerId = 1,
            };

            await this.repository.AddAsync(job);
            await repository.SaveChangesAsync();

            var model = new EditJobViewModel()
            {
                Title = "Job Title",
                Description = "Job Description",
                Requirements = "Job Requirements",
                Responsibilities = "Job Responsibilities",
                TownName = "Job Town",
                WorkingTime = "Full-Time",
                Salary = 1000,
                Offering = "Job Offering",
                CategoryId = 1
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await this.jobService.Update(6, model));
            Assert.That(ex.Message, Is.EqualTo("The requested job was not found."));
        }

        [Test]
        public async Task UpdateShoulWorkCorrectly()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);


            var job = new Job()
            {
                Id = 1,
                Description = "",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "Town 1",
                Title = "",
                IsDeleted = false,
                WorkingTime = "Remote",
                EmployerId = 1,
            };

            await this.repository.AddAsync(job);
            await repository.SaveChangesAsync();

            await this.jobService.Update(1, new EditJobViewModel()
            {
                Id = 1,
                Title = "Job Title",
                Description = "Job Description",
                Requirements = "Job Requirements",
                Responsibilities = "Job Responsibilities",
                TownName = "Job Town",
                WorkingTime = "Full-Time",
                Salary = 1000,
                Offering = "Job Offering",
                CategoryId = 1
            });

            var dbJob = await this.repository.GetByIdAsync<Job>(1);

            Assert.That(dbJob.Title, Is.EqualTo("Job Title"));
        }

        [Test]
        public async Task GetAllJobsMethodWithParametersTest()
        {
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job()
                {
                    Id = 1,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 1",
                    Title = "",
                    IsDeleted = false,
                    Category = new Category { Id = 1,  Name = "test"},
                    WorkingTime = "Remote",
                    EmployerId = 1,
                    IsApproved = true,
                     CreatedOn = new DateTime(2022, 3, 1)
                },
                 new Job()
                {
                    Id = 2,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 2",
                    Title = "",
                    IsDeleted = false,
                    IsApproved = true,
                    Category = new Category { Id = 2,  Name = "test2"},
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                     CreatedOn = new DateTime(2022, 2, 1)
                },
                   new Job()
                {
                    Id = 3,
                    Description = "",
                    Offering = "",
                    Requirements = "",
                    Responsibilities = "",
                    Town = "Town 3",
                    Title = "",
                    IsDeleted = false,
                    Category = new Category { Id = 3,  Name = "test3"},
                    IsApproved = true,
                    WorkingTime = "Part-Time",
                    EmployerId= 1,
                    CreatedOn = new DateTime(2022, 1, 1)
                }
            });

            await this.repository.SaveChangesAsync();

            var jobs = await this.jobService.GetAllJobs(1, 2);

            Assert.That(jobs, Is.Not.Null);
            Assert.That(jobs.Count, Is.EqualTo(2));
            Assert.That(jobs.First().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetJobsWithCVReturn0WhenEmployerIdIsWrong()
        {
            //var categoryServiceMock = new Mock<ICategoryService>();
            //var categoryService = categoryServiceMock.Object;

            //this.repository = new DbRepository(this.context);
            //this.jobService = new JobService(this.repository, categoryService);

            //var userId = "testUser";
            //var employerId = 1;

            //var employer = new Employer
            //{
            //    Id = employerId,
            //    FirstName = "",
            //    LastName = "",
            //    UserId = userId
            //};

            //var job1 = new Job()
            //{
            //    Id = 1,
            //    Description = "Job 1",
            //    Offering = "",
            //    Requirements = "",
            //    Responsibilities = "",
            //    Town = "",
            //    Title = "Job 1",
            //    CategoryId = 1,
            //    IsDeleted = false,
            //    EmployerId = employerId
            //};

            //var cv1 = new CV
            //{
            //    Id = 1,
            //    EmployeeId = 1,
            //    Jobs = new List<Job> { job1 },
            //    IsDeleted = false,
            //    Address = "",
            //    Education = "",
            //    Experience = "",
            //    Gender = "",
            //    PhoneNumber = "",
            //    ResumeFile = Array.Empty<byte>(),
            //    ResumeFileName = "",
            //    Skills = "",
            //    Summary = "",
            //    Title = "",
            //};

            //await this.repository.AddAsync(employer);
            //await this.repository.AddAsync(job1);
            //await this.repository.AddAsync(cv1);
            //await this.repository.SaveChangesAsync();

            //var result = await jobService.GetJobsWithCV(new JobsWithCVsViewModel(), userId);

            //Assert.Multiple(() =>
            //{
            //    Assert.That(result, Is.Not.Null);
            //    Assert.That(result.Count(), Is.EqualTo(1));
            //    Assert.That(result.First().CVs, Has.Count.EqualTo(1));
            //});
            var categoryServiceMock = new Mock<ICategoryService>();
            var categoryService = categoryServiceMock.Object;

            this.repository = new DbRepository(this.context);
            this.jobService = new JobService(this.repository, categoryService);

            var userId = "testUser";
            var employerId = 1;

            var employer = new Employer
            {
                Id = employerId,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = "damm"
            };

            var job1 = new Job()
            {
                Id = 1,
                Description = "Job 1",
                Offering = "",
                Requirements = "",
                Responsibilities = "",
                Town = "",
                Title = "Job 1",
                CategoryId = 1,
                IsDeleted = false,
                EmployerId = employerId
            };

            var cv1 = new CV
            {
                Id = 1,
                Employee = employee,
                Jobs = new List<Job> { job1 },
                IsDeleted = false,
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
            };

            await this.repository.AddAsync(employer);
            await this.repository.AddAsync(job1);
            await this.repository.AddAsync(cv1);
            await this.repository.SaveChangesAsync();

            var result = await jobService.GetJobsWithCV(new JobsWithCVsViewModel(), userId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().CVs, Has.Count.EqualTo(1));
            });
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }
    }
}