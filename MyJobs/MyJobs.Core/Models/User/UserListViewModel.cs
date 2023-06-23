namespace MyJobs.Core.Models.User
{
    using System.ComponentModel.DataAnnotations;
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Role name")]
        public string RoleName { get; set; }
    }
}
