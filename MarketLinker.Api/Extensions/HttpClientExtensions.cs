using System.Net.Http.Headers;

namespace MarketLinker.Api.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddMarketplaceHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("MercadoLivre", client =>
        {
            client.BaseAddress = new Uri("https://api.mercadolibre.com/");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
        return services;
    }
}