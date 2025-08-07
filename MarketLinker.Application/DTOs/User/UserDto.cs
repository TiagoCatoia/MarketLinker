namespace MarketLinker.Application.DTOs.User;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; set; } = null!;
}