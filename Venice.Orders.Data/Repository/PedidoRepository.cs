using Venice.Domain.Entities;
using Venice.Orders.Domain.Interfaces;
using MongoDB.Driver;
using Venice.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Venice.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidoSqlDbContext _sqlContext;
        private readonly IMongoCollection<ItemPedido> _mongoCollection;

        public PedidoRepository(PedidoSqlDbContext sqlContext, PedidoMongoDbContext mongoContext)
        {
            _sqlContext = sqlContext;
            _mongoCollection = mongoContext.ItensPedido;
        }

        public async Task CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            if (itens == null || !itens.Any())
                throw new ArgumentException("Pedido precisa ter ao menos um item.");

            _sqlContext.Pedidos.Add(pedido);
            await _sqlContext.SaveChangesAsync();

            foreach (var item in itens)
                item.PedidoId = pedido.Id;

            await _mongoCollection.InsertManyAsync(itens);
        }

        public async Task<Pedido?> ObterPorIdAsync(Guid id)
        {
            return await _sqlContext.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ItemPedido>> ObterItensPorPedidoIdAsync(Guid pedidoId)
        {
            return await _mongoCollection.Find(x => x.PedidoId == pedidoId).ToListAsync();
        }
    }

}
