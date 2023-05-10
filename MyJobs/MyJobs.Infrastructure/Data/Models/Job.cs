namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;

    public class Job
    {
        public Job()
        {
            this.Employees = new HashSet<Employee>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.JobTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.CompanyNameMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.RequirementsMaxLength)]
        public string Requirements { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.OfferingMaxLength)]
        public string Offering { get; set; } = null!;

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int EmployerId { get; set; }
        public Employer Employer { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
