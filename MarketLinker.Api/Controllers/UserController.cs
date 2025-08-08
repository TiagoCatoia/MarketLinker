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
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        User.ValidateUserIdOrThrow(id);
        
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found.");

        var userDto = UserMapper.ToDto(user);
        return Ok(userDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, CancellationToken cancellationToken)
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        User.ValidateUserIdOrThrow(id);
        
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found.");
        
        await _userRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}