using MongoDB.Driver;
using Venice.Domain.Entities;

namespace Venice.Data.Context
{
    public class PedidoMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public PedidoMongoDbContext(string connectionString, string databaseName)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(databaseName);
            
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<ItemPedido> ItensPedido =>
            _database.GetCollection<ItemPedido>("ItensPedido");
    }
}
