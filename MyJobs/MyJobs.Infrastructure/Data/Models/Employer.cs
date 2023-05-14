namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class Employer
    {
        public Employer()
        {
            this.Jobs = new HashSet<Job>();
            this.EmployeeEmployments = new HashSet<EmployeeEmployment>();
        }

        [Key]
        public int EmployerId { get; set; }

        [Required]
        [MaxLength(DataConstants.EmployerFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.EmployerLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        //public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<EmployeeEmployment> EmployeeEmployments { get; set; }

    }
}
