namespace MyJobs.Core.Models.Job
{
    public class JobsListViewModel
    {
        public IEnumerable<JobsViewModel>? Jobs { get; set; }
        public bool HasPreviousPage => this.PageNumber > 1;
        public int PreviousPageNumber => this.PageNumber - 1;
        public bool HasNextPage => this.PageNumber < PagesCount;
        public int NextPageNumber => this.PageNumber + 1;
        public int PagesCount => (int)Math.Ceiling((double)this.JobsTotalCount / this.ItemsPerPage);
        public int PageNumber { get; set; }
        public int JobsTotalCount { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
