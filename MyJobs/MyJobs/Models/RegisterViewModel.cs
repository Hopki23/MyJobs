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
        [MaxLength(DataConstants.UserUsernameMaxLength)]
        [MinLength(DataConstants.UserUsernameMinLength)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.UserFirstNameMaxLength)]
        [MinLength(DataConstants.UserFirstNameMinLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.UserLastNameMaxLength)]
        [MinLength(DataConstants.UserLastNameMinLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        public bool IsEmployer { get; set; }
    }
}
