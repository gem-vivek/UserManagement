using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public int UserId { get; set; } // Add this property for UserId

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; } // Property to hold the token
    }
}
