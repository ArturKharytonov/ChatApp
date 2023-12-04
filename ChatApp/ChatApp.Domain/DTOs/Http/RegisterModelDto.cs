using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.DTOs.Http
{
    public class RegisterModelDto
    {
        [Required]
        [Display(Name = "UserName")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
