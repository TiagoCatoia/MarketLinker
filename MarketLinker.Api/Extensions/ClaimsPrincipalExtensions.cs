using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MarketLinker.Domain.Exceptions;

namespace MarketLinker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(sub, out var guid) ? guid : null;
    }
    
    public static void ValidateUserIdOrThrow(this ClaimsPrincipal user, Guid id)
    {
        var userIdClaim = GetUserId(user);
        if (userIdClaim is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (userIdClaim != id)
            throw new ForbiddenException("Access denied.");
    }
}