using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Models;


namespace EventManagementSystem.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}