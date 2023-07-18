﻿namespace MyJobs.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;

    using NUnit.Framework;
    using Moq;

    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Data.Models.Identity;
    using MyJobs.Infrastructure.Models;
    using MyJobs.Core.Models.Profile;
    using MyJobs.Core.Services;

    [TestFixture]
    public class ProfileServiceTests
    {
        private MyJobsDbContext context;
        private IDbRepository repository;

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
        public async Task EditProfileShouldUpdateEmployeeProfile()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser { Id = userId, UserName = "f00", Email = "f00@example.com", PasswordHash = "test",
            FirstName = "", LastName = ""};

            userMgr.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var roles = new List<string> { "Employee" };
            userMgr.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            userMgr.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var employeeId = 1;

            var employee = new Employee
            {
                Id = employeeId,
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = new UserProfileViewModel
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com"
            };

            await profileService.EditProfile(model, employeeId, userId);

            employee = await this.repository.All<Employee>()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            Assert.Multiple(() =>
            {
                Assert.That(employee!.FirstName, Is.EqualTo("Jane"));
                Assert.That(employee!.LastName, Is.EqualTo("Smith"));
                Assert.That(employee!.User.Email, Is.EqualTo("jane@example.com"));
            });
        }

        [Test]
        public async Task EditProfileShouldThrowExceptionForNotExistingEmployee()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "f00",
                Email = "f00@example.com",
                PasswordHash = "test",
                FirstName = "",
                LastName = ""
            };

            userMgr.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var roles = new List<string> { "Employee" };
            userMgr.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            userMgr.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var employeeId = 1;

            var employee = new Employee
            {
                Id = employeeId,
                UserId = "faaaakeeee",
                FirstName = "John",
                LastName = "Doe"
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = new UserProfileViewModel
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.EditProfile(model, employeeId, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employee"));
        }

        [Test]
        public async Task EditProfileShouldUpdateEmployerProfile()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "f00",
                Email = "f00@example.com",
                PasswordHash = "test",
                FirstName = "",
                LastName = ""
            };

            userMgr.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var roles = new List<string> { "Employer" };
            userMgr.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            userMgr.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var employerId = 1;

            var employer = new Employer
            {
                Id = employerId,
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                User = user,
                Company = new Company { Id = 1, CompanyName = "test", Address = "da", PhoneNumber = "sakflhjhjfj"}
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var model = new UserProfileViewModel
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                CompanyName = "aidai"
            };

            await profileService.EditProfile(model, employerId, userId);

            employer = await this.repository.All<Employer>()
                .FirstOrDefaultAsync(e => e.Id == employerId);

            Assert.Multiple(() =>
            {
                Assert.That(employer!.FirstName, Is.EqualTo("Jane"));
                Assert.That(employer!.LastName, Is.EqualTo("Smith"));
                Assert.That(employer!.User.Email, Is.EqualTo("jane@example.com"));
                Assert.That(employer!.Company.CompanyName, Is.EqualTo("aidai"));
            });
        }

        [Test]
        public async Task EditProfileShouldThrowExceptionForNotExistingEmployer()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "f00",
                Email = "f00@example.com",
                PasswordHash = "test",
                FirstName = "",
                LastName = ""
            };

            userMgr.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var roles = new List<string> { "Employer" };
            userMgr.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            userMgr.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var employerId = 1;

            var employer = new Employer
            {
                Id = employerId,
                UserId = "fakeee",
                FirstName = "John",
                LastName = "Doe",
                Company = new Company { Id = 1, CompanyName = "test", Address = "da", PhoneNumber = "sakflhjhjfj" }
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var model = new UserProfileViewModel
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                CompanyName = "aidai"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.EditProfile(model, employerId, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employee"));
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }
    }
}
