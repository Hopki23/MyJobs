namespace MyJobs.Core.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using MyJobs.Infrastructure.Constants;
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [Required]
        [Compare(nameof(PasswordRepeat))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password Repeat")]
        public string PasswordRepeat { get; set; } = null!;

        [Required]
        [MinLength(UserConstants.UserUsernameMinLength)]
        [MaxLength(UserConstants.UserUsernameMaxLength)]
        public string Username { get; set; } = null!;

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

        [Display(Name = "User Type")]
        public string? UserType { get; set; }

        [Display(Name = "Company name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}

