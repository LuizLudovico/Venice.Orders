using MongoDB.Driver;
using Venice.Domain.Entities;

namespace Venice.Data.Context
{
    public class PedidoDbContext
    {
        private readonly IMongoDatabase _database;

        public PedidoDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<Pedido> Pedidos => _database.GetCollection<Pedido>("Pedidos");
    }
}
