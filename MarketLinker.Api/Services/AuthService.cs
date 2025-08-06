using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MarketLinker.Application.DTOs.Auth;
using MarketLinker.Domain.Entities.Marketplace.Auth;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MarketLinker.Api.Services;

public class AuthService : IAuthService
{
    private readonly MarketLinkerDbContext  _dbContext;
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public AuthService(
        IConfiguration config,
        IRefreshTokenRepository refreshTokenRepo,
        MarketLinkerDbContext  context)
    {
        _config = config;
        _refreshTokenRepo = refreshTokenRepo;
        _dbContext = context;
    }
    
    public string GenerateAccessToken(string userId)
    {
        const string jwtKey = "Jwt:Key";
        const string jwtIssuer = "Jwt:Issuer";
        const string jwtAudience = "Jwt:Audience";
            
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config[jwtKey] ?? throw new InvalidOperationException($"Config '{jwtKey}' missing")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config[jwtIssuer],
            audience: _config[jwtAudience],
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string deviceName, CancellationToken cancellationToken)
    {
        await using var transactional = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var tokenInDb = await _refreshTokenRepo.GetByTokenAsync(refreshToken, cancellationToken);
        
        if (tokenInDb is null || tokenInDb.IsRevoked || tokenInDb.IsExpired())
            throw new SecurityTokenException("Invalid refresh token");
        
        var userId = tokenInDb.UserId;
        
        var tokenResponse =  await GenerateAndSaveTokensAsync(userId, deviceName, cancellationToken);
        
        await transactional.CommitAsync(cancellationToken);
        
        return tokenResponse;
    }

    public async Task<TokenResponseDto> GenerateAndSaveTokensAsync(Guid userId, string deviceName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            throw new InvalidOperationException("Device name could not be identified.");
        
        deviceName = deviceName.Trim().ToLower();
        
        var newAccessToken = GenerateAccessToken(userId.ToString());
        var newRefreshToken = GenerateRefreshToken();
        
        await _refreshTokenRepo.RevokeLastDeviceTokenAsync(userId, deviceName, cancellationToken);
        
        await _refreshTokenRepo.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = userId,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            DeviceName = deviceName
        }, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}