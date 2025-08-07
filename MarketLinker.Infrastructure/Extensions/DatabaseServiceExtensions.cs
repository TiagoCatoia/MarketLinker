using MarketLinker.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketLinker.Infrastructure.Extensions;

public static class DatabaseServiceExtensions
{
    public static SqliteConnection AddSqliteInMemoryDatabase(this IServiceCollection services)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        services.AddSingleton(connection);
        services.AddDbContext<MarketLinkerDbContext>(opt => opt.UseSqlite(connection));

        return connection;
    }
}