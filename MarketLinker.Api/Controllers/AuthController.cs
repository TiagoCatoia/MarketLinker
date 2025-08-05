using MarketLinker.Api.Services;
using MarketLinker.Application.DTOs.Login;
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
    public async Task<IActionResult> Login(LoginRequestDto requestDto)
    {
        var user = await _userRepository.GetByEmailAsync(requestDto.Email);
        if (user is null || user.CheckPassword(requestDto.Password))
            return Unauthorized(new { message = "Invalid credentials"});
        
        var token = _authService.GenerateToken(user.Id.ToString());
        var responseDto = new LoginResponseDto(token);
        return Ok(responseDto);
    }
}