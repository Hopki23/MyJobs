namespace MyJobs.Infrastructure.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;

    public class Category
    {
        public Category()
        {
            this.Jobs = new HashSet<Job>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(JobConstants.CategoryMaxLenght)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
