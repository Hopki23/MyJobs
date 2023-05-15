namespace MyJobs.Core.Services
{
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

        public IndexViewModel GetCategories()
        {
            var viewModel = new IndexViewModel
            {
                Categories = dbRepository.AllReadonly<Category>()
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
