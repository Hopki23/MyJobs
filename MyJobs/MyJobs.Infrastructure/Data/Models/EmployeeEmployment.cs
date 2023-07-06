namespace MyJobs.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
  
    using System.ComponentModel.DataAnnotations.Schema;
    public class EmployeeEmployment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [ForeignKey(nameof(Employer))]
        public int EmployerId { get; set; }
        public virtual Employer? Employer { get; set; }

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }
}
