namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;

    public class CV
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.SummaryMaxLength)]
        public string Summary { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.EducationMaxLength)]
        public string Education { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.ExperienceMaxLength)]
        public string Experience { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.SkillsMaxLength)]
        public string Skills { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
