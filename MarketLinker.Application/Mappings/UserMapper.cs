using MarketLinker.Application.DTOs.User;
using MarketLinker.Domain.Entities.User;

namespace MarketLinker.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email
        };
    }
    
    
}