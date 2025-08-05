using UserEntity = MarketLinker.Domain.Entities.User.User;

namespace MarketLinker.Domain.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}