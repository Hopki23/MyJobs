namespace MyJobs.Core.Models.Job
{
    public class SingleJobViewModel
    {
        public string Title { get; set; } = null!;
        public string Town { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Requirements { get; set; } = null!;
        public string Offering { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string CompanyName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string EmployerFirstName { get; set; } = null!;
        public string EmployerLastName { get; set; } = null!;
    }
}
