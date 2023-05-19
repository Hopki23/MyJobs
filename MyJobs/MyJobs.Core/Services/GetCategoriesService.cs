namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using MyJobs.Core.Models.Category;
    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Data.Models;

    public class GetCategoriesService : IGetCategoriesService
    {
        private readonly IDbRepository dbRepository;

        public GetCategoriesService(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllCategories()
        {
            return dbRepository.AllReadonly<Category>()
                    .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name))
                    .ToList();
        }

        public IndexViewModel GetCategories()
        {
           return new IndexViewModel
            {
                Categories = this.dbRepository.AllReadonly<Category>()
                .Select(c => new CategoryViewModel
                {
                    CategoryName = c.Name,
                    JobCount = c.Jobs.Count
                })
                .ToList()
            };
        }
    }
}
