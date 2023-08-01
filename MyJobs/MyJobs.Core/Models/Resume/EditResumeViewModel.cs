namespace MyJobs.Core.Models.Resume
{
    using System.ComponentModel.DataAnnotations;
    using MyJobs.Infrastructure.Constants;
    public class EditResumeViewModel
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsPictureRemoved { get; set; }

        [Required]
        [MinLength(CVConstants.TitleMinLength)]
        [MaxLength(CVConstants.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(CVConstants.SummaryMinLength)]
        [MaxLength(CVConstants.SummaryMaxLength)]
        public string Summary { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        [MinLength(CVConstants.EducationMinLength)]
        [MaxLength(CVConstants.EducationMaxLength)]
        public string Education { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.ExperienceMaxLength)]
        public string Experience { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.SkillsMaxLength)]
        public string Skills { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.AddressMaxLength)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(CVConstants.PhoneNumberMaxLength)]

        public string PhoneNumber { get; set; } = null!;
    }
}
