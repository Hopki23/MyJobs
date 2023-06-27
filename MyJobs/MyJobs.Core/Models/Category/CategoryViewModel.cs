namespace MyJobs.Core.Models.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int JobCount { get; set; }
    }
}