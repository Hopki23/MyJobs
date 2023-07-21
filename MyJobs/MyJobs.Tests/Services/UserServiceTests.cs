namespace MyJobs.Tests.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Data.Models.Identity;

    [TestFixture]
    public class UserServiceTests
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
        public async Task DisableUserAsyncShouldWorkCorrectly()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);

            this.repository = new DbRepository(this.context);
            var userService = new UserService(this.repository, userMgr.Object);

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "f00",
                Email = "f00@example.com",
                PasswordHash = "test",
                FirstName = "",
                LastName = "",
                IsDeleted = false
            };

            await this.repository.AddAsync(user);
            await this.repository.SaveChangesAsync();

            await userService.DisableUserAsync(userId);

            var dbUser = await this.repository.GetByIdAsync<ApplicationUser>(userId);
            Assert.That(dbUser.IsDeleted, Is.True);
        }

        [Test]
        public async Task EnableUserAsyncShouldWorkCorrectly()
        {
            var userId = "testUser";
            var UserStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            var userMgr = new Mock<UserManager<ApplicationUser>>(UserStoreMock, null, null, null, null, null, null, null, null);

            this.repository = new DbRepository(this.context);
            var userService = new UserService(this.repository, userMgr.Object);

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "f00",
                Email = "f00@example.com",
                PasswordHash = "test",
                FirstName = "",
                LastName = "",
                IsDeleted = true
            };

            await this.repository.AddAsync(user);
            await this.repository.SaveChangesAsync();

            await userService.EnableUserAsync(userId);

            var dbUser = await this.repository.GetByIdAsync<ApplicationUser>(userId);
            Assert.That(dbUser.IsDeleted, Is.False);
        }

        [Test]
        public async Task GetAllUsersShouldWorkCorrectly()
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

            var roles = new List<string> { RoleConstants.Employee };
            userMgr.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            userMgr.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            this.repository = new DbRepository(this.context);
            var userService = new UserService(this.repository, userMgr.Object);

            await this.repository.AddAsync(user);
            await this.repository.SaveChangesAsync();

            var users = await userService.GetAllUsers();

            Assert.That(users.Count, Is.EqualTo(1));
        }
    }
}

//public async Task<IEnumerable<UserListViewModel>> GetAllUsers()
//{
//    var users = await this.repository.All<ApplicationUser>()
//        .Select(u => new UserListViewModel()
//        {
//            Id = u.Id,
//            Email = u.Email,
//            FirstName = u.FirstName,
//            LastName = u.LastName,
//            Username = u.UserName,
//            IsDisabled = u.IsDeleted
//        })
//        .ToListAsync();

//    foreach (var user in users)
//    {
//        var appUser = await this.userManager.FindByIdAsync(user.Id);
//        var role = await this.userManager.GetRolesAsync(appUser);

//        user.RoleName = role.FirstOrDefault()!;
//    }

//    return users;
//}