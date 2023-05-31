namespace MyJobs.Core.Models.Job
{
    public class CreateJobViewModel : BaseJobViewModel
    {
        public DateTime CreatedOn { get; set; }
        public int EmployerId { get; set; }
    }
}
