using DotNetEnv;
using MarketLinker.Api.Configuration;
using MarketLinker.Api.Extensions;
using MarketLinker.Api.Middleware;
using MarketLinker.Api.Services;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Extensions;
using MarketLinker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Services (DI)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure SQLite In-Memory Database
builder.Services.AddSqliteInMemoryDatabase();

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

var app = builder.Build();

// Ensure a database is created
app.EnsureDatabaseCreated();

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