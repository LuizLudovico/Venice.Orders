using Venice.Domain.Entities;

namespace Venice.Orders.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens);
}
