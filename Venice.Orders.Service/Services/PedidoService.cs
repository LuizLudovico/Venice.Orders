using Venice.Domain.Entities;
using Venice.Domain.Events;
using Venice.Domain.Interfaces.Bus;
using Venice.Domain.Interfaces.Cache;
using Venice.Domain.Interfaces.Services;
using Venice.Orders.Domain.Interfaces;
using Venice.Service.DTOs;

namespace Venice.Service.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMessageBus _messageBus;
        private readonly IRedisCacheService _cacheService;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IMessageBus messageBus,
            IRedisCacheService cacheService)
        {
            _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Pedido> CriarPedidoAsync(Pedido pedido, List<ItemPedido> itens)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            if (itens == null || !itens.Any())
                throw new ArgumentException("Pedido precisa ter ao menos um item.", nameof(itens));

            await _pedidoRepository.CriarPedidoAsync(pedido, itens);

            // Publica no RabbitMQ
            var evento = new PedidoCriadoEvent(pedido.Id, pedido.ClienteId, pedido.Data);
            await _messageBus.PublicarPedidoCriadoAsync(evento);

            // Remove do cache (se existir)
            await _cacheService.RemoveAsync($"pedido:{pedido.Id}");

            return pedido;
        }

        public async Task<(Pedido pedido, List<ItemPedido> itens)> ObterPedidoCompletoAsync(Guid id)
        {
            string cacheKey = $"pedido:{id}";

            var cache = await _cacheService.GetAsync<PedidoCompletoCacheDTO>(cacheKey);
            if (cache != null)
            {
                return (cache.Pedido, cache.Itens);
            }

            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido com ID {id} não encontrado.");

            var itens = await _pedidoRepository.ObterItensPorPedidoIdAsync(id);

            var dto = new PedidoCompletoCacheDTO
            {
                Pedido = pedido,
                Itens = itens
            };

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(2));

            return (pedido, itens);
        }
    }
}
