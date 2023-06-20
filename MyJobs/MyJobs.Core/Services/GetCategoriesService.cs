namespace MyJobs.Core.Services
{
    using System.Collections.Generic;
    
    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Data.Models;

    public class GetCategoriesService : IGetCategoriesService
    {
        private readonly IDbRepository dbRepository;

        public GetCategoriesService(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetAllCategories()
        {
            return await this.dbRepository.AllReadonly<Category>()
                    .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name))
                    .ToListAsync();
        }

        public async Task<IndexViewModel> GetCategories()
        {
            Dictionary<string, string> categoryIcons = GetCategoryIcons();

            return new IndexViewModel
            {
                Categories = await this.dbRepository.AllReadonly<Category>()
                 .Select(c => new CategoryViewModel
                 {
                     CategoryName = c.Name,
                     JobCount = c.Jobs.Count,
                     IconClass = categoryIcons.ContainsKey(c.Name) ? categoryIcons[c.Name] : null
                 })
                 .OrderByDescending(x => x.JobCount)
                 .ToListAsync()
            };
        }

        private Dictionary<string, string> GetCategoryIcons()
        {
            Dictionary<string, string> categoryIcons = new()
            {
                { "Software Engineer", "fa-brain" },
                { "Food and Hospitality", "fa-pizza-slice" },
                { "Aviation and Aerospace", "fa-plane" },
                { "Real Estate","fa-hotel" },
                { "Education and Training", "fa-book-open" },
                { "Marketing and Advertising", "fa-ad" },
                { "Healthcare and Medical", "fa-notes-medical" },
                { "Full-Stack Developer", "fa-terminal" },
                { "Back-End Developer", "fa-code" },
                { "Front-End Developer", "fa-cube" },
                { "QA Tester", "fa-bug" }
            };

            return categoryIcons;
        }
    }
}
