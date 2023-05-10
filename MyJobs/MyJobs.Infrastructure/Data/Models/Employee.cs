namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class Employee
    {
        public Employee()
        {
            this.CVs = new HashSet<CV>();
            this.Jobs = new HashSet<Job>();
        }

        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(DataConstants.EmployeeFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.EmployeeLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int EmployerId { get; set; }
        public Employer Employer { get; set; } = null!;
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public virtual ICollection<CV> CVs { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
