namespace MarketLinker.Api.Extensions;

public static class SessionExtensions
{
    public static IServiceCollection AddAppSession(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        
        return services;
    }
}