﻿namespace MyJobs.Infrastructure.Models
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
        [MaxLength(JobConstants.OfferingMaxLength)]
        public string Offering { get; set; } = null!;
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int EmployerId { get; set; }
        public Employer Employer { get; set; } = null!;

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
