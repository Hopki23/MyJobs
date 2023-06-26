namespace MyJobs.Core.Models.Category
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    public class AddCategoryViewModel
    {
        [Required]
        [MinLength(CategoryConstants.CategoryMinLenght, ErrorMessage = "Category must be with minimum length {1}.")]
        [MaxLength(CategoryConstants.CategoryMaxLenght, ErrorMessage = "Category must be with maximum length {1}.")]
        public string Name { get; set; } = null!;
    }
}
