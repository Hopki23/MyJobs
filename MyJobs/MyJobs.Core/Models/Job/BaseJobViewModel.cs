namespace MyJobs.Core.Models.Job
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    
    using MyJobs.Infrastructure.Constants;
    public abstract class BaseJobViewModel
    {
        [Required]
        [MaxLength(JobConstants.JobTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(JobConstants.JobDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(JobConstants.RequirementsMaxLength)]
        public string Requirements { get; set; } = null!;
        [Required]
        [MaxLength(JobConstants.JobResponsibilitiesMaxLength)]
        public string Responsibilities { get; set; } = null!;

        [Required]
        [MaxLength(JobConstants.TownNameMaxLength)]
        [DisplayName("Town Name")]
        public string TownName { get; set; } = null!;

        [DisplayName("Working Time")]
        public string? WorkingTime { get; set; }

        [Range(300, 100000)]
        public decimal? Salary { get; set; }

        [Required]
        [MaxLength(JobConstants.OfferingMaxLength)]
        public string Offering { get; set; } = null!;

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public IEnumerable<KeyValuePair<string, string>>? CategoryItems { get; set; }
    }
}
