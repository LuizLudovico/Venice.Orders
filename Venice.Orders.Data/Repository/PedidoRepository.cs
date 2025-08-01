using Venice.Domain.Entities;
using Venice.Orders.Domain.Interfaces;
using MongoDB.Driver;

namespace Venice.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IMongoCollection<Pedido> _collection;

        public PedidoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Pedido>("Pedidos");
        }

        public async Task CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens)
        {
            pedido.AdicionarItens(itens);
            await _collection.InsertOneAsync(pedido);
        }

        public async Task<Pedido?> ObterPorIdAsync(Guid id)
        {
            return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }
    }
}
