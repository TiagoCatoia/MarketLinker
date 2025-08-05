using MarketLinker.Api.Configuration;
using MarketLinker.Api.Services;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using MarketLinker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<MarketLinkerDbContext>(opt => opt.UseInMemoryDatabase("MarketLinkerDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();