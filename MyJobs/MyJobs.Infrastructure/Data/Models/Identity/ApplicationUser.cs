namespace MyJobs.Infrastructure.Data.Models.Identity
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;
    using MyJobs.Infrastructure.Constants;

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(DataConstants.UserFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.UserLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
