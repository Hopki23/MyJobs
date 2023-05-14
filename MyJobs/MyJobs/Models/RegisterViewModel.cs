namespace MyJobs.Models
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
        [MinLength(DataConstants.UserUsernameMinLength)]
        [MaxLength(DataConstants.UserUsernameMaxLength)]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(DataConstants.UserFirstNameMinLength)]
        [MaxLength(DataConstants.UserFirstNameMaxLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(DataConstants.UserLastNameMinLength)]
        [MaxLength(DataConstants.UserLastNameMaxLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Display(Name = "User Type")]
        public string? UserType { get; set; } 

        [Display(Name = "Skills")]
        public string? Skills { get; set; }

        [Display(Name = "Company name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}

