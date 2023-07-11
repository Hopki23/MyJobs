namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;

    public class CategoryService : ICategoryService
    {
        private readonly IDbRepository dbRepository;

        public CategoryService(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await this.dbRepository.AddAsync(category);
            await this.dbRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await this.dbRepository.All<Category>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                throw new ArgumentException("The requested category was not found.");
            }

            category.IsDeleted = true;

            await this.dbRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryJobsAsync(int id)
        {
            var jobs = await this.GetJobsByCategoryId(id);

            foreach (var job in jobs)
            {
                job.IsDeleted = true;
            }

            await this.dbRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAll()
        {
            return await this.dbRepository.AllReadonly<Category>()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                    IsDeleted = c.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetAllCategories()
        {
            return await this.dbRepository.AllReadonly<Category>()
                    .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name))
                    .ToListAsync();
        }

        public async Task<IndexViewModel> GetCategories()
        {
            return new IndexViewModel
            {
                Categories = await this.dbRepository.AllReadonly<Category>()
                 .Where(x => !x.IsDeleted)
                 .Select(c => new CategoryViewModel
                 {
                     CategoryName = c.Name,
                     JobCount = c.Jobs.Count(j => !j.IsDeleted && j.IsApproved)
                 })
                 .OrderByDescending(x => x.JobCount)
                 .ToListAsync()
            };
        }

        public async Task RestoreCategoryAsync(int id)
        {
            var category = await this.dbRepository.All<Category>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                throw new ArgumentException("The requested category was not found.");
            }

            category.IsDeleted = false;

            await this.dbRepository.SaveChangesAsync();
        }

        public async Task RestoreCategoryJobsAsync(int id)
        {
            var jobs = await this.GetJobsByCategoryId(id);

            foreach (var job in jobs)
            {
                job.IsDeleted = false;
            }

            await this.dbRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByCategoryId(int id)
        {
            return await this.dbRepository.All<Job>()
                   .Where(j => j.CategoryId == id)
                   .ToListAsync();
        }

        public async Task<bool> CategoryExistById(int id)
        {
            return await this.dbRepository.AllReadonly<Category>()
                .AnyAsync(c => c.Id == id);
        }
    }
}
