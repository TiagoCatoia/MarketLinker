using System.ComponentModel.DataAnnotations;

namespace MarketLinker.Application.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; init; } = null!;
    
    [Required(ErrorMessage = "Device name is required.")]   
    public string DeviceName { get; init; } = null!;
}