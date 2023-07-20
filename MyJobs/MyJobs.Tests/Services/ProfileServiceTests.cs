namespace MyJobs.Tests.Services
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
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Constants;

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

        [Test]
        public async Task GetReadNotificationsTest()
        {
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var userId = "dasjk";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var notification = new Notification()
            {
                Id = 1,
                EmployeeId = employee.Id,
                Message = "da",
                IsRead = true,
                EmployerId = employer.Id
            };

            var notification2 = new Notification()
            {
                Id = 2,
                EmployeeId = employee.Id,
                Message = "daaa",
                IsRead = true,
                EmployerId = employer.Id
            };

            employer.Notifications.Add(notification);
            employer.Notifications.Add(notification2);
            employee.Notifications.Add(notification);
            employee.Notifications.Add(notification2);

            await this.repository.AddAsync(employer);
            await this.repository.AddAsync(employee);
            await this.repository.AddAsync(notification);
            await this.repository.AddAsync(notification2);
            await this.repository.SaveChangesAsync();

            var notifications = await profileService.GetReadNotifications(userId);

            Assert.That(notifications, Is.Not.Null);
            Assert.That(notifications.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetUnreadNotificationsTest()
        {
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var userId = "dasjk";

            var employer = new Employer
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "",
                LastName = "",
                UserId = userId
            };

            var notification = new Notification()
            {
                Id = 1,
                EmployeeId = employee.Id,
                Message = "da",
                IsRead = false,
                EmployerId = employer.Id
            };

            var notification2 = new Notification()
            {
                Id = 2,
                EmployeeId = employee.Id,
                Message = "daaa",
                IsRead = false,
                EmployerId = employer.Id
            };

            employer.Notifications.Add(notification);
            employer.Notifications.Add(notification2);
            employee.Notifications.Add(notification);
            employee.Notifications.Add(notification2);

            await this.repository.AddAsync(employer);
            await this.repository.AddAsync(employee);
            await this.repository.AddAsync(notification);
            await this.repository.AddAsync(notification2);
            await this.repository.SaveChangesAsync();

            var notifications = await profileService.GetUnreadNotifications(userId);

            Assert.That(notifications, Is.Not.Null);
            Assert.That(notifications.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task MarkNotificationAsRead()
        {
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);

            this.repository = new DbRepository(this.context);
            var profileService = new ProfileService(this.repository, userMgr.Object);

            var notification = new Notification()
            {
                Id = 1,
                Message = "da",
                IsRead = false,
            };

            await this.repository.AddAsync(notification);
            await this.repository.SaveChangesAsync();

            await profileService.MarkNotificationAsRead(1);

            var dbNotification = await this.repository.GetByIdAsync<Notification>(1);

            Assert.That(dbNotification.IsRead, Is.True);
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForNotExistingEmployee()
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
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(5, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employee!"));
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForNotExistingRole()
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

            var roles = new List<string> { };
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
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(1, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid role!"));
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForEmployeeWrongUserId()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser
            {
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
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(employeeId, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employee!"));
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForNotWrongRole()
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

            var roles = new List<string> { "Greshka" };
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
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(1, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid role!"));
        }

         [Test]
        public async Task GetProfileForEditingShouldWorkCorrectlyForEmployee()
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
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                User = user
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();

            var model = await profileService.GetProfileForEditing(employeeId, userId);

            Assert.Multiple(() =>
            {
                Assert.That(model.FirstName, Is.EqualTo("John"));
                Assert.That(model.LastName, Is.EqualTo("Doe"));
            });
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForEmployerWrongUserId()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ApplicationUser
            {
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
                UserId = "faaaakeeee",
                FirstName = "John",
                LastName = "Doe",
                User = user,
                Company = new Company { Id = 1, Address = "", CompanyName = "", PhoneNumber = ""}
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(employerId, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employer!"));
        }

        [Test]
        public async Task GetProfileForEditingShouldThrowExceptionForNotExistingEmployer()
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
                UserId = "faaaakeeee",
                FirstName = "John",
                LastName = "Doe",
                User = user,
                Company = new Company { Id = 1, Address = "", CompanyName = "", PhoneNumber = "" }
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await profileService.GetProfileForEditing(5, userId));
            Assert.That(ex.Message, Is.EqualTo("Invalid employer!"));
        }

        [Test]
        public async Task GetProfileForEditingShouldWorkCorrectlyForEmployer()
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
                Company = new Company { Id = 1, Address = "dasdsa", CompanyName = "aneve", PhoneNumber = "amida" }
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var model = await profileService.GetProfileForEditing(employerId, userId);

            Assert.Multiple(() =>
            {
                Assert.That(model.FirstName, Is.EqualTo("John"));
                Assert.That(model.LastName, Is.EqualTo("Doe"));
                Assert.That(model.CompanyName, Is.EqualTo("aneve"));
                Assert.That(model.IsEmployee, Is.False);
            });
        }

        [Test]
        public async Task GetUserByIdShouldWorkCorrectlyForEmployee()
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

            var model = await profileService.GetUserById(userId, RoleConstants.Employee);

            Assert.Multiple(() =>
            {
                Assert.That(model.Id, Is.EqualTo(1));
                Assert.That(model.FirstName, Is.EqualTo("John"));
                Assert.That(model.LastName, Is.EqualTo("Doe"));
            });
        }

        [Test]
        public async Task GetUserByIdShouldWorkCorrectlyForEmployer()
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
                Company = new Company { Id = 1, CompanyName = "test", Address = "da", PhoneNumber = "sakflhjhjfj" }
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var model = await profileService.GetUserById(userId, RoleConstants.Employer);

            Assert.Multiple(() =>
            {
                Assert.That(model.Id, Is.EqualTo(1));
                Assert.That(model.FirstName, Is.EqualTo("John"));
                Assert.That(model.CompanyName, Is.EqualTo("test"));
                Assert.That(model.CompanyAddress, Is.EqualTo("da"));
            });
        }

        [Test]
        public async Task GetUserByIdShouldThrowExceptionForInvalidRole()
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
                Company = new Company { Id = 1, CompanyName = "test", Address = "da", PhoneNumber = "sakflhjhjfj" }
            };

            await this.repository.AddAsync(employer);
            await this.repository.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await profileService.GetUserById(userId, RoleConstants.Administrator));
            Assert.That(ex.Message, Is.EqualTo(ErrorConstants.InvalidUserRole));
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }
    }
}

//public async Task<UserProfileViewModel> GetUserById(string id, string role)
//{
//    var userProfile = new UserProfileViewModel();

//    if (role == RoleConstants.Employee)
//    {
//        var employee = await this.repository
//            .AllReadonly<Employee>()
//            .Include(e => e.User)
//            .Where(e => e.UserId == id.ToString())
//            .FirstOrDefaultAsync();

//        userProfile.Id = employee!.Id;
//        userProfile.FirstName = employee!.FirstName;
//        userProfile.LastName = employee!.LastName;
//        userProfile.Email = employee.User.Email;
//        userProfile.IsEmployee = true;
//    }
//    else if (role == RoleConstants.Employer)
//    {
//        var employer = await this.repository.AllReadonly<Employer>()
//            .Include(e => e.Company)
//            .Include(e => e.User)
//            .Where(e => e.UserId == id.ToString())
//            .FirstOrDefaultAsync();
//        if (employer == null)
//        {
//            throw new InvalidOperationException();
//        }

//        userProfile.Id = employer!.Id;
//        userProfile.FirstName = employer.FirstName;
//        userProfile.LastName = employer.LastName;
//        userProfile.Email = employer.User.Email;
//        userProfile.CompanyName = employer.Company.CompanyName;
//        userProfile.CompanyAddress = employer.Company.Address;
//        userProfile.CompanyPhoneNumber = employer.Company.PhoneNumber;
//    }
//    else
//    {
//        throw new InvalidOperationException(ErrorConstants.InvalidUserRole);
//    }

//    return userProfile;
//}