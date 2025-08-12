using System.Text;
using MarketLinker.Api.Extensions;
using MarketLinker.Application.Interfaces.ExternalClients;
using MarketLinker.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketLinker.Api.Controllers;

[ApiController]
[Route("api/mercadolivre")]
[Authorize]
public class MercadoLivreController : ControllerBase
{
    private readonly IMercadoLivreApiClient _mlApiClient;
    private readonly IConfiguration _config;

    public MercadoLivreController(IMercadoLivreApiClient mlApiClient, IConfiguration config)
    {
        _mlApiClient = mlApiClient;
        _config = config;
    }

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var currentUserId = User.GetUserId();
        if (currentUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");
        
        var state = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{currentUserId}|{Guid.NewGuid()}")
        );

        HttpContext.Session.SetString("oauth_state", state);

        var clientId = _config["MercadoLivre:Client_Id"];
        var redirectUri = _config["MercadoLivre:Redirect_Uri"];
        var urlTemplate = _config["MercadoLivre:MercadoLivreAuthorizationUrl"];

        if (string.IsNullOrEmpty(urlTemplate) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
        {
            throw new InvalidOperationException(
                "Invalid MercadoLivre configuration. Please check ClientId, RedirectUri, and Authorization URL.");
        }

        var authorizationUrl = urlTemplate
            .Replace("{clientId}", clientId)
            .Replace("{redirectUri}", Uri.EscapeDataString(redirectUri))
            .Replace("{state}", state);

        return Redirect(authorizationUrl);
    }

    [HttpGet("oauth-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        var expecedState = HttpContext.Session.GetString("oauth_state");
        _mlApiClient.ValidateState(state, expecedState);
        
        if (string.IsNullOrEmpty(code))
            throw new BadRequestException("Missing code parameter.");
        
        var currentUserId = User.GetUserId();
        if (currentUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var tokenData = await _mlApiClient.ExchangeCodeForTokenAsync(code, cancellationToken);
        await _mlApiClient.SaveAuthDataAsync(currentUserId.Value, tokenData, cancellationToken);

        return Ok("Account linked successfully.");
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        if (currentUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");
        
        var tokenData = await _mlApiClient.RefreshTokenAsync(currentUserId.Value, cancellationToken);
        await _mlApiClient.SaveAuthDataAsync(currentUserId.Value, tokenData, cancellationToken);
        
        return Ok("Token refreshed successfully.");
    }
}