using MarketLinker.Infrastructure.Data;

namespace MarketLinker.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void EnsureDatabaseCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketLinkerDbContext>();
        db.Database.EnsureCreated();
    }
}