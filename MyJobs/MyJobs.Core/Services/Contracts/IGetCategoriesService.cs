namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Category;
    public interface IGetCategoriesService
    {
        IndexViewModel GetCategories();
        IEnumerable<KeyValuePair<string, string>> GetAllCategories();
    }
}
