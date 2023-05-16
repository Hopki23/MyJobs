namespace MyJobs.Core.Services
{
    using MyJobs.Core.Models.Category;
    public interface IGetCategoriesService
    {
        IndexViewModel GetCategories();
        IEnumerable<KeyValuePair<string, string>> GetAllCategories();
    }
}
