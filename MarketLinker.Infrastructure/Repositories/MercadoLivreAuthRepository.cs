using MarketLinker.Domain.Entities.Marketplace.Auth;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketLinker.Infrastructure.Repositories;

public class MercadoLivreAuthRepository : BaseRepository<MercadoLivreAuth>, IMercadoLivreAuthRepository
{
    public MercadoLivreAuthRepository(MarketLinkerDbContext dbContext) : base(dbContext) { }

    public async Task<MercadoLivreAuth?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.MarketplaceAuths
            .OfType<MercadoLivreAuth>()
            .FirstOrDefaultAsync(auth => auth.UserId == userId, cancellationToken);
    }
}   