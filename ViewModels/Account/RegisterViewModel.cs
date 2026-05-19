using System.ComponentModel.DataAnnotations;

namespace project_itransition.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Password is required.")]
        [StringLength(40,MinimumLength =8,ErrorMessage ="The {0} must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage ="Confirm password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
