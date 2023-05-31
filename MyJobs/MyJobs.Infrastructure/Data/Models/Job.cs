namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models;

    public class Job
    {
        public Job()
        {
            this.Employees = new HashSet<Employee>();
            this.Resumes = new HashSet<CV>();
        }

        [Key]
        public int Id { get; set; }

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
        public string Town { get; set; } = null!;
        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(JobConstants.OfferingMaxLength)]
        public string Offering { get; set; } = null!;
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        public string? WorkingTime { get; set; }
        public decimal? Salary { get; set; }
        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public int EmployerId { get; set; }
        public Employer Employer { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<CV> Resumes { get; set; }
    }
}
