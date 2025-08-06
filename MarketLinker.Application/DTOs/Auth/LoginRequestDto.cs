﻿using System.ComponentModel.DataAnnotations;

namespace MarketLinker.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = null!;
}