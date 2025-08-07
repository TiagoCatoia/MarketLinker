using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarketLinker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(sub, out var guid) ? guid : null;
    }
}