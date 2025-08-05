using MarketLinker.Api.Configuration;
using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<MarketLinkerDbContext>(opt => opt.UseInMemoryDatabase("PersonDb"));

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