namespace MyJobs.Core.Models.Job
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    public class CreateJobViewModel
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
        [MaxLength(JobConstants.OfferingMaxLength)]
        public string Offering { get; set; } = null!;
        public int CompanyId { get; set; }
        public int EmployerId { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<KeyValuePair<string, string>>? CategoryItems { get; set; }
    }
}
