using System.IdentityModel.Tokens.Jwt;
using MarketLinker.Api.Extensions;
using MarketLinker.Application.DTOs.User;
using MarketLinker.Application.Mappings;
using MarketLinker.Domain.Entities.User;
using MarketLinker.Domain.Exceptions;
using MarketLinker.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketLinker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.GetUserId();
        if (userIdClaim is null || userIdClaim != id)
            return Forbid();
        
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found.");

        var userDto = UserMapper.ToDto(user);
        return Ok(userDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(registerUserDto.Email, cancellationToken);
        if (user is not null)
            throw new ConflictException("Email already registered.");

        user = new User
        {
            Email = registerUserDto.Email
        };
        user.SetPassword(registerUserDto.Password);

        await _userRepository.AddAsync(user, cancellationToken);

        var userDto = UserMapper.ToDto(user);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, userDto);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.GetUserId();
        if (userIdClaim is null || userIdClaim != id)
            return Forbid();
        
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found.");
        
        await _userRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}