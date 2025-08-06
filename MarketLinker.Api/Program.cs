using MarketLinker.Api.Configuration;
using MarketLinker.Api.Middleware;
using MarketLinker.Api.Services;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using MarketLinker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Services (DI)
builder.Services.AddDbContext<MarketLinkerDbContext>(opt => opt.UseInMemoryDatabase("MarketLinkerDb"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

var app = builder.Build();

// HTTPS redirection
app.UseHttpsRedirection();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Exception handling
app.UseMiddleware<ExceptionMiddleware>();

// Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.Run();