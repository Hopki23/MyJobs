namespace MyJobs.Tests
{
    using Microsoft.EntityFrameworkCore;
    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;

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

        [Test]
        public async Task DeleteCategoryJobsAsyncTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

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

            await this.repository.AddAsync(category);
            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            await this.categoryService.DeleteCategoryJobsAsync(1);

            var dbJob = await this.repository.GetByIdAsync<Job>(1);

            Assert.That(dbJob.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteCategoryJobsAsyncShouldDeleteMultipleJobs()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 2, IsDeleted = false },
            });

            await this.repository.AddAsync(category);
            await this.repository.SaveChangesAsync();

            await this.categoryService.DeleteCategoryJobsAsync(1);


            var updatedJobs = await this.repository.AllReadonly<Job>().ToListAsync();

            foreach (var job in updatedJobs.Where(j => j.CategoryId == 1))
            {
                Assert.That(job.IsDeleted, Is.True);
            }
        }

        [Test]
        public async Task DeleteCategoryJobsAsyncShouldNotDeleteTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

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

            await this.repository.AddAsync(category);
            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();

            await this.categoryService.DeleteCategoryJobsAsync(2);

            var dbJob = await this.repository.GetByIdAsync<Job>(2);

            Assert.That(dbJob, Is.Null);
        }

        [Test]
        public async Task GetAllTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = true },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
            });

            await this.repository.SaveChangesAsync();

            var allCategories = await this.categoryService.GetAll();

            Assert.That(allCategories.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllReturnsEmptyTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            var allCategories = await this.categoryService.GetAll();

            Assert.That(allCategories, Is.Empty);
        }

        [Test]
        public async Task GetAllCategoriesTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
            });

            await this.repository.SaveChangesAsync();

            var result = await this.categoryService.GetAllCategories();

            foreach (var category in result)
            {
                var keyValue = result.FirstOrDefault(kvp => kvp.Key == category.Key.ToString());

                Assert.Multiple(() =>
                {
                    Assert.That(result.Count, Is.EqualTo(3));
                    Assert.That(keyValue.Key, Is.EqualTo(category.Key.ToString()));
                    Assert.That(keyValue.Value, Is.EqualTo(category.Value));
                });
            }
        }

        [Test]
        public async Task GetCategoriesTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = true, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = true, IsApproved = false },
                 new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 3, IsDeleted = true, IsApproved = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 4, IsDeleted = true, IsApproved = false },
            });

            await this.repository.SaveChangesAsync();

            var result = await this.categoryService.GetCategories();

            var expectedCategory = new CategoryViewModel { CategoryName = "Category 1", JobCount = 2 };
            var actualCategory = result.Categories.FirstOrDefault(c => c.CategoryName == expectedCategory.CategoryName);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Categories, Has.Count.EqualTo(3));

                Assert.That(actualCategory, Is.Not.Null);
                Assert.That(actualCategory.JobCount, Is.EqualTo(expectedCategory.JobCount));
            });
        }

        [Test]
        public async Task GetCategoriesShouldReturn0Test()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = true },
                new Category { Id = 2, Name = "Category 2", IsDeleted = true },
                new Category { Id = 3, Name = "Category 3", IsDeleted = true },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await this.repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true }
            });

            await this.repository.SaveChangesAsync();

            var result = await this.categoryService.GetCategories();

            Assert.That(result.Categories, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task RestoreCategoryAsyncThrowsExceptionTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await this.repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await this.categoryService.RestoreCategoryAsync(5));
            Assert.That(exception.Message, Is.EqualTo("The requested category was not found."));
        }

        [Test]
        public async Task RestoreCategoryAsyncTest()
        {
            this.repository = new DbRepository(this.context);
            this.categoryService = new CategoryService(this.repository);

            await this.repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await this.repository.SaveChangesAsync();

            await this.categoryService.RestoreCategoryAsync(2);

            var restoredCategory = await this.repository.GetByIdAsync<Category>(2);

            Assert.That(restoredCategory, Is.Not.Null);
            Assert.That(restoredCategory.IsDeleted, Is.False);
        }
    }
}