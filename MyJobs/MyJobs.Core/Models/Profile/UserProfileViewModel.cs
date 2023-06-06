namespace MyJobs.Core.Models.Profile
{
    public class UserProfileViewModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public bool IsEmployee { get; set; }
    }
}
