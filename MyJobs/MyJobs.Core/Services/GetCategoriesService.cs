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
            //return this.dbRepository.AllReadonly<Category>()
            //     .Select(c => new
            //     {
            //         c.Id,
            //         c.Name
            //     })
            //     .ToList()
            //     .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name));
            return dbRepository.AllReadonly<Category>()
                    .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name))
                    .ToList();
        }

        public IndexViewModel GetCategories()
        {
            var viewModel = new IndexViewModel
            {
                Categories = this.dbRepository.AllReadonly<Category>()
                .Select(c => new CategoryViewModel
                {
                    CategoryName = c.Name,
                    JobCount = c.Jobs.Count
                })
                .ToList()
            };

            return viewModel;
        }
    }
}
