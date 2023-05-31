﻿namespace MyJobs.Core.Models.Job
{
    public class SingleJobViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Town { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Requirements { get; set; } = null!;
        public string  Responsibilities { get; set; } = null!;
        public string Offering { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string CompanyName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string EmployerFirstName { get; set; } = null!;
        public string EmployerLastName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? WorkingTime { get; set; } 
        public decimal? Salary { get; set; }
        public bool IsOwner { get; set; }
    }
}
