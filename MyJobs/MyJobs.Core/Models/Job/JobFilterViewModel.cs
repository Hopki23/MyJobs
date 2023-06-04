namespace MyJobs.Core.Models.Job
{
    using MyJobs.Infrastructure.Models;

    public class JobFilterViewModel
    {
        public List<KeyValuePair<string, string>> Categories { get; set; }
        public int SelectedCategoryId { get; set; }

        public List<string> WorkingTimes { get; set; }
        public List<string> SelectedWorkingTimes { get; set; }

        public List<string> TownNames { get; set; }
        public List<string> SelectedTownNames { get; set; }

        public List<Job> FilteredJobs { get; set; }
    }
}
