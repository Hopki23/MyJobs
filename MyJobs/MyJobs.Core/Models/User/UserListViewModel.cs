namespace MyJobs.Core.Models.User
{
    using System.ComponentModel.DataAnnotations;
    public class UserListViewModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [Display(Name = "Role name")]
        public string RoleName { get; set; } = null!;

        public bool IsDisabled { get; set; }
    }
}
