using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<MarketLinkerDbContext>(opt => opt.UseInMemoryDatabase("PersonDb"));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();