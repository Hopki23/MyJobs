namespace MyJobs.Tests
{
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Data.Models;

    [TestFixture]
    public class CategoryServiceTests
    {
        private MyJobsDbContext context;
        private IDbRepository repository;
        private ICategoryService categoryService;

        [SetUp]
        public void SetUp()
        {
            var contextOptions = new DbContextOptionsBuilder<MyJobsDbContext>()
                .UseInMemoryDatabase("MyJobs")
                .Options;

            this.context = new MyJobsDbContext(contextOptions);
            this.context.Database.EnsureDeleted();
            this.context.Database.EnsureCreated();
        }

        [Test]
        public async Task AddCategoryTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.categoryService.AddCategoryAsync(new Category
            {
                Id = 1,
                IsDeleted = false,
                Name = "Test"
            });

            await this.repository.SaveChangesAsync();

            var dbCategory = await this.repository.GetByIdAsync<Category>(1);

            Assert.That(dbCategory, Is.Not.Null);
            Assert.That(dbCategory.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task DeleteCategoryTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>()
            {
                new Category() { Id = 1, Name = "Test", IsDeleted = false },
                new Category() { Id = 2, Name = "Test 2", IsDeleted = false },
                new Category() { Id = 3, Name = "Test 3", IsDeleted = false },
            });

            await this.repository.SaveChangesAsync();

            await this.categoryService.DeleteCategoryAsync(1);

            var dbCategory = await this.repository.GetByIdAsync<Category>(1);

            Assert.That(dbCategory.IsDeleted, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteCategoryShouldThrowException()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>()
            {
                new Category() { Id = 1, Name = "Test", IsDeleted = false },
                new Category() { Id = 2, Name = "Test 2", IsDeleted = false },
                new Category() { Id = 3, Name = "Test 3", IsDeleted = false },
            });

            await this.repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await this.categoryService.DeleteCategoryAsync(5));
            Assert.That(exception.Message, Is.EqualTo("The requested category was not found."));
        }
    }
}
