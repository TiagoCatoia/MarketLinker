using System.ComponentModel.DataAnnotations;

namespace MarketLinker.Application.DTOs.User;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; init; } = null!;
}