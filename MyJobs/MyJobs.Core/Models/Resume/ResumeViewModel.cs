namespace MyJobs.Core.Models.Resume
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    public class ResumeViewModel
    {
        public string? Image { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

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
        [MaxLength(CVConstants.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.SkillsMaxLength)]
        public string Skills { get; set; } = null!;
    }
}
