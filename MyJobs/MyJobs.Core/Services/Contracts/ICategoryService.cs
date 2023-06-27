namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Category;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Models;

    public interface ICategoryService
    {
        Task<IndexViewModel> GetCategories();

        Task<IEnumerable<KeyValuePair<string, string>>> GetAllCategories();

        Task<IEnumerable<Job>> GetJobsByCategoryId(int id);

        Task AddCategoryAsync(Category category); 

        Task DeleteCategoryAsync(int id);

        Task RestoreCategoryAsync(int id);

        Task RestoreCategoryJobsAsync(int id);

        Task<IEnumerable<CategoryViewModel>> GetAll();

        Task DeleteCategoryJobsAsync(int id);
    }
}
