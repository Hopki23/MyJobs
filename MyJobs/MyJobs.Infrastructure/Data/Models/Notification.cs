namespace MyJobs.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyJobs.Infrastructure.Models;

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [ForeignKey(nameof(Employer))]
        public int EmployerId { get; set; }
        public virtual Employer Employer { get; set; }
    }
}
