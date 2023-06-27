namespace MyJobs.Core.Services
{
    using System.Collections.Generic;
    
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models;

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
                 .Select(c => new CategoryViewModel
                 {
                     CategoryName = c.Name,
                     JobCount = c.Jobs.Count,
                 })
                 .OrderByDescending(x => x.JobCount)
                 .ToListAsync()
            };
        }
    }
}
