namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;

    public class CV
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CVConstants.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.SummaryMaxLength)]
        public string Summary { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.EducationMaxLength)]
        public string Education { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.ExperienceMaxLength)]
        public string Experience { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.SkillsMaxLength)]
        public string Skills { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
