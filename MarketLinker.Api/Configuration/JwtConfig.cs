using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MarketLinker.Api.Configuration;

public static class JwtConfig
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
    {
        var keyString = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured. Please define 'Jwt:Key' in your appsettings.json, environment variables, or .env file.");
        var key = Encoding.UTF8.GetBytes(keyString);
        var issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Issuer is not configured. Please define 'Jwt:Issuer' in your appsettings.json, environment variables, or .env file.");
        var audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Audience is not configured. Please define 'Jwt:Audience' in your appsettings.json, environment variables, or .env file.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.RequireHttpsMetadata = !env.IsDevelopment();
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });
        
        return services;
    }
}