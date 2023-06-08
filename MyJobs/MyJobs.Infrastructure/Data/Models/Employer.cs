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
            this.Notifications = new HashSet<Notification>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EmployerConstants.EmployerFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(EmployerConstants.EmployerLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<EmployeeEmployment> EmployeeEmployments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
