namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Data.Models;
    using MyJobs.Infrastructure.Data.Models.Identity;

    public class Employee
    {
        public Employee()
        {
            this.CVs = new HashSet<CV>();
            this.Jobs = new HashSet<Job>();
            this.Employments = new HashSet<EmployeeEmployment>();
            this.Notifications = new HashSet<Notification>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EmployeeConstants.EmployeeFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(EmployeeConstants.EmployeeLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public virtual ICollection<CV> CVs { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<EmployeeEmployment> Employments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
