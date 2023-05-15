namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;

    public class Company
    {
        public Company()
        {
            this.Employers = new HashSet<Employer>();
            this.EmployeeEmployments = new HashSet<EmployeeEmployment>();
            this.Jobs = new HashSet<Job>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CompanyConstants.CompanyNameMaxLength)]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(CompanyConstants.PhoneNumberMaxLenght)]
        public string PhoneNumber { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Employer> Employers { get; set; }
        public virtual ICollection<EmployeeEmployment> EmployeeEmployments { get; set; }
    }
}
