using MyJobs.Infrastructure.Constants;
using System.ComponentModel.DataAnnotations;

namespace MyJobs.Core.Models.Profile
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(UserConstants.UserFirstNameMinLength)]
        [MaxLength(UserConstants.UserFirstNameMaxLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(UserConstants.UserLastNameMinLength)]
        [MaxLength(UserConstants.UserLastNameMaxLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MinLength(CompanyConstants.CompanyNameMinLength)]
        [MaxLength(CompanyConstants.CompanyNameMaxLength)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [MinLength(CompanyConstants.CompanyAddressMinLength)]
        [MaxLength(CompanyConstants.CompanyAddressMaxLength)]
        [Display(Name = "Company Address")]
        public string? CompanyAddress { get; set; }

        [MaxLength(CompanyConstants.PhoneNumberMaxLenght)]
        [Display(Name = "Company Phone number")]
        public string? CompanyPhoneNumber { get; set; }
        public bool IsEmployee { get; set; }
    }
}
