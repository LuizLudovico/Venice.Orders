using Venice.Domain.Entities;

namespace Venice.Domain.Interfaces.Repository
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(Guid id); // SQL Server
        Task<List<ItemPedido>> ObterItensPorPedidoIdAsync(Guid pedidoId); // MongoDB
        Task CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens); // SQL + Mongo
    }
}
