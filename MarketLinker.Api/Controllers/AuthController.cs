using MarketLinker.Api.Services;
using MarketLinker.Application.DTOs.Auth;
using MarketLinker.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarketLinker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
        
    public AuthController(IUserRepository userRepository, IAuthService authService)
    {
        this._userRepository = userRepository;
        this._authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto requestDto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(requestDto.Email, cancellationToken);
        if (user is null || !user.CheckPassword(requestDto.Password))
            return Unauthorized(new { message = "Invalid credentials"});
        
        var tokenResponse = await _authService.GenerateAndSaveTokensAsync(user.Id, cancellationToken);
        return Ok(tokenResponse);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request ,CancellationToken cancellationToken)
    {
        var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(tokenResponse);
    }
}