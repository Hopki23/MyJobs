namespace MyJobs.Tests.Services
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

            context = new MyJobsDbContext(contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public async Task AddCategoryTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await categoryService.AddCategoryAsync(new Category
            {
                Id = 1,
                IsDeleted = false,
                Name = "Test"
            });

            await repository.SaveChangesAsync();

            var dbCategory = await repository.GetByIdAsync<Category>(1);

            Assert.That(dbCategory, Is.Not.Null);
            Assert.That(dbCategory.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task DeleteCategoryTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>()
            {
                new Category() { Id = 1, Name = "Test", IsDeleted = false },
                new Category() { Id = 2, Name = "Test 2", IsDeleted = false },
                new Category() { Id = 3, Name = "Test 3", IsDeleted = false },
            });

            await repository.SaveChangesAsync();

            await categoryService.DeleteCategoryAsync(1);

            var dbCategory = await repository.GetByIdAsync<Category>(1);

            Assert.That(dbCategory.IsDeleted, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteCategoryShouldThrowException()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>()
            {
                new Category() { Id = 1, Name = "Test", IsDeleted = false },
                new Category() { Id = 2, Name = "Test 2", IsDeleted = false },
                new Category() { Id = 3, Name = "Test 3", IsDeleted = false },
            });

            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await categoryService.DeleteCategoryAsync(5));
            Assert.That(exception.Message, Is.EqualTo("The requested category was not found."));
        }

        [Test]
        public async Task DeleteCategoryJobsAsyncTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

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

            await repository.AddAsync(category);
            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            await categoryService.DeleteCategoryJobsAsync(1);

            var dbJob = await repository.GetByIdAsync<Job>(1);

            Assert.That(dbJob.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteCategoryJobsAsyncShouldDeleteMultipleJobs()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 2, IsDeleted = false },
            });

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            await categoryService.DeleteCategoryJobsAsync(1);


            var updatedJobs = await repository.AllReadonly<Job>().ToListAsync();

            foreach (var job in updatedJobs.Where(j => j.CategoryId == 1))
            {
                Assert.That(job.IsDeleted, Is.True);
            }
        }

        [Test]
        public async Task DeleteCategoryJobsAsyncShouldNotDeleteTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

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

            await repository.AddAsync(category);
            await repository.AddAsync(job);
            await repository.SaveChangesAsync();

            await categoryService.DeleteCategoryJobsAsync(2);

            var dbJob = await repository.GetByIdAsync<Job>(2);

            Assert.That(dbJob, Is.Null);
        }

        [Test]
        public async Task GetAllTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = true },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
            });

            await repository.SaveChangesAsync();

            var allCategories = await categoryService.GetAll();

            Assert.That(allCategories.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllReturnsEmptyTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var allCategories = await categoryService.GetAll();

            Assert.That(allCategories, Is.Empty);
        }

        [Test]
        public async Task GetAllCategoriesTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
            });

            await repository.SaveChangesAsync();

            var result = await categoryService.GetAllCategories();

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
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = true, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = true, IsApproved = false },
                 new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 3, IsDeleted = true, IsApproved = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 4, IsDeleted = true, IsApproved = false },
            });

            await repository.SaveChangesAsync();

            var result = await categoryService.GetCategories();

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
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = true },
                new Category { Id = 2, Name = "Category 2", IsDeleted = true },
                new Category { Id = 3, Name = "Category 3", IsDeleted = true },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false, IsApproved = true }
            });

            await repository.SaveChangesAsync();

            var result = await categoryService.GetCategories();

            Assert.That(result.Categories, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task RestoreCategoryAsyncThrowsExceptionTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await repository.SaveChangesAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await categoryService.RestoreCategoryAsync(5));
            Assert.That(exception.Message, Is.EqualTo("The requested category was not found."));
        }

        [Test]
        public async Task RestoreCategoryAsyncTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            await repository.AddRangeAsync(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsDeleted = false },
                new Category { Id = 2, Name = "Category 2", IsDeleted = false },
                new Category { Id = 3, Name = "Category 3", IsDeleted = false },
                new Category { Id = 4, Name = "Category 4", IsDeleted = true },
            });

            await repository.SaveChangesAsync();

            await categoryService.RestoreCategoryAsync(2);

            var restoredCategory = await repository.GetByIdAsync<Category>(2);

            Assert.That(restoredCategory, Is.Not.Null);
            Assert.That(restoredCategory.IsDeleted, Is.False);
        }

        [Test]
        public async Task RestoreCategoryJobsAsyncTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 2, IsDeleted = false },
            });

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            await this.categoryService.RestoreCategoryJobsAsync(1);

            var restoredJobs = await this.repository.AllReadonly<Job>()
                .Where(j => j.CategoryId == 1)
                .ToListAsync();

            foreach (var job in restoredJobs)
            {
                Assert.That(job, Is.Not.Null);
                Assert.That(job.IsDeleted, Is.False);
            }
        }

        [Test]
        public async Task RestoreCategoryJobsAsyncShouldReturnEmptyCollctionTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 2, IsDeleted = false },
            });

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            await this.categoryService.RestoreCategoryJobsAsync(5);

            var restoredJobs = await this.repository.AllReadonly<Job>()
                .Where(j => j.CategoryId == 5)
                .ToListAsync();

            Assert.That(restoredJobs, Is.Empty);
            Assert.That(restoredJobs, Is.Not.Null);
        }

        [Test]
        public async Task GetJobsByCategoryIdTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddRangeAsync(new List<Job>()
            {
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 1, IsDeleted = false },
                new Job { Description = "", Offering="",Requirements="",Responsibilities="",Town="",Title= "", CategoryId = 2, IsDeleted = false },
            });

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            await this.categoryService.GetJobsByCategoryId(1);

            var jobs = await this.repository.AllReadonly<Job>()
               .Where(j => j.CategoryId == 1)
               .ToListAsync();

            Assert.That(jobs, Is.Not.Null);
            Assert.That(jobs, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task CategoryExistByIdShouldReturnTrueTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddAsync(category);
            await this.repository.SaveChangesAsync();

            var result = await this.categoryService.CategoryExistById(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CategoryExistByIdShouldReturnFalseTest()
        {
            repository = new DbRepository(context);
            categoryService = new CategoryService(repository);

            var category = new Category()
            {
                Id = 1,
                Name = "Test",
                IsDeleted = false
            };

            await repository.AddAsync(category);
            await this.repository.SaveChangesAsync();

            var result = await this.categoryService.CategoryExistById(2);

            Assert.That(result, Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }
    }
}