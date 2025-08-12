using DotNetEnv;
using MarketLinker.Api.Configuration;
using MarketLinker.Api.Extensions;
using MarketLinker.Api.Middleware;
using MarketLinker.Api.Services;
using MarketLinker.Application.Interfaces.ExternalClients;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Extensions;
using MarketLinker.Infrastructure.Integrations;
using MarketLinker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Enable both HTTP and HTTPS for local testing
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5292);
    options.ListenLocalhost(7052, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Load environment variables
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IMercadoLivreAuthRepository, MercadoLivreAuthRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// External Clients
builder.Services.AddScoped<IMercadoLivreApiClient, MercadoLivreApiClient>();

// Configure SQLite In-Memory Database
builder.Services.AddSqliteInMemoryDatabase();

// Controllers and Swagger and Extensions
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddAppSession();
builder.Services.AddLogging();

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

// Session
app.UseSession();

// Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.Run();