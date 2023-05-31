namespace MyJobs.Core.Models.Category
{
    public class CategoryViewModel
    {
        //[Required]
        //[MinLength(JobConstants.CategoryMinLenght)]
        //[MaxLength(JobConstants.CategoryMaxLenght)]
        public string CategoryName { get; set; } = null!;
        public int JobCount { get; set; }
        public string IconClass { get; set; }
    }
}
