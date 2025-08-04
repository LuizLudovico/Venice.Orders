using Venice.Domain.Entities;

namespace Venice.Application.DTOs
{
    public class PedidoCompletoCacheDTO
    {
        public Pedido Pedido { get; set; } = null!;
        public List<ItemPedido> Itens { get; set; } = new();
    }
}
