using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}