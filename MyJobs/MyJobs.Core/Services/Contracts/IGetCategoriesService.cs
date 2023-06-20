namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Category;
    public interface IGetCategoriesService
    {
        Task<IndexViewModel> GetCategories();
        Task<IEnumerable<KeyValuePair<string, string>>> GetAllCategories();
    }
}
