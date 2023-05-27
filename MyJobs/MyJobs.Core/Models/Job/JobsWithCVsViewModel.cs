namespace MyJobs.Core.Models.Job
{
    using MyJobs.Infrastructure.Models;
    public class JobsWithCVsViewModel
    {
        public Job Job { get; set; }
        public List<CV> CVs { get; set; }
    }
}
