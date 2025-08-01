using Venice.Domain.Entities;
using Venice.Domain.Events;
using Venice.Domain.Interfaces.Bus;
using Venice.Domain.Interfaces.Services;
using Venice.Orders.Domain.Interfaces;

namespace Venice.Service.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMessageBus _messageBus;

        public PedidoService(IPedidoRepository pedidoRepository, IMessageBus messageBus)
        {
            _pedidoRepository = pedidoRepository;
            _messageBus = messageBus;
        }

        public async Task<Pedido> CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens)
        {
            await _pedidoRepository.CriarPedidoAsync(pedido, itens);

            // Publicar evento no RabbitMQ
            var evento = new PedidoCriadoEvent(pedido.Id, pedido.ClienteId, pedido.Data);
            await _messageBus.PublicarPedidoCriadoAsync(evento);

            return pedido;
        }

        public async Task<(Pedido pedido, List<ItemPedido> itens)> ObterPedidoCompletoAsync(Guid id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                throw new Exception("Pedido não encontrado.");

            // Como você está usando Mongo, pode ser que os itens estejam dentro da entidade Pedido
            // ou precise implementar a persistência dos itens em outro repositório/coleção, se necessário.
            // Neste exemplo, assumo que `pedido` já contém os itens (ajuste conforme necessário).

            var itens = new List<ItemPedido>(); // ajuste se seus dados armazenarem os itens
            return (pedido, itens);
        }
    }
}
