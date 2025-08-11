using MarketLinker.Domain.Entities.Marketplace.Auth;

namespace MarketLinker.Domain.Repositories;

public interface IMercadoLivreAuthRepository : IBaseRepository<MercadoLivreAuth>
{
    Task<MercadoLivreAuth?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}