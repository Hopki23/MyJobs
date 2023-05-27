namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;

    public class CV
    {
        public CV()
        {
            this.Jobs = new HashSet<Job>();
        }

        [Key]
        public int Id { get; set; }
        public string? Image { get; set; }

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
        [Required]
        public byte[] ResumeFile { get; set; } = null!;

        [Required]
        public string ResumeFileName { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public virtual ICollection<Job> Jobs { get; set; }
    }
}