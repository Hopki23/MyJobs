namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Category;
    using MyJobs.Infrastructure.Data.Models;

    public interface ICategoryService
    {
        Task<IndexViewModel> GetCategories();
        Task<IEnumerable<KeyValuePair<string, string>>> GetAllCategories();
        Task AddCategoryAsync(Category category); 
        Task DeleteCategoryAsync(int id);
    }
}
