using System.Threading.Tasks;
using Npgsql;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public abstract class PgRepository : IPgRepository 
{
    private readonly string _connectionString;
    protected PgRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected async Task<NpgsqlConnection> GetConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Due to in-process migrations
        connection.ReloadTypes();
        
        return connection;
    }
}