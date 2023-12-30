using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.DTOs.Http.Requests.Users;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current Password is required")]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}