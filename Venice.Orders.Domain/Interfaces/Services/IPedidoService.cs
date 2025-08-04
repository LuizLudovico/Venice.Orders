using Venice.Domain.Entities;

namespace Venice.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens);
        Task<(Pedido pedido, List<ItemPedido> itens)> ObterPedidoCompletoAsync(Guid id);
    }
}
