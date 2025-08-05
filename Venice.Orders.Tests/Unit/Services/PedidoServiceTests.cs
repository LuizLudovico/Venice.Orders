using FluentAssertions;
using Moq;
using Venice.Domain.Entities;
using Venice.Domain.Events;
using Venice.Domain.Interfaces.Bus;
using Venice.Domain.Interfaces.Cache;
using Venice.Domain.Interfaces.Repository;
using Venice.Service.Services;
using Xunit;

namespace Venice.Orders.Tests.Unit.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _mockRepository;
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly Mock<IRedisCacheService> _mockCacheService;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _mockRepository = new Mock<IPedidoRepository>();
        _mockMessageBus = new Mock<IMessageBus>();
        _mockCacheService = new Mock<IRedisCacheService>();
        _service = new PedidoService(_mockRepository.Object, _mockMessageBus.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task CriarPedidoAsync_ComPedidoValido_DeveCriarPedidoComSucesso()
    {
        // Arrange
        var pedido = Pedido.Criar(1, DateTime.Now, "Pendente");
        var itens = new List<ItemPedido>
        {
            new() { Produto = "Produto 1", Quantidade = 2, PrecoUnitario = 10.50m },
            new() { Produto = "Produto 2", Quantidade = 1, PrecoUnitario = 25.00m }
        };

        _mockRepository.Setup(x => x.CriarPedidoAsync(pedido, itens))
            .Returns(Task.CompletedTask);
        _mockMessageBus.Setup(x => x.PublicarPedidoCriadoAsync(It.IsAny<PedidoCriadoEvent>()))
            .Returns(Task.CompletedTask);
        _mockCacheService.Setup(x => x.RemoveAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CriarPedidoAsync(pedido, itens);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(pedido);
        _mockRepository.Verify(x => x.CriarPedidoAsync(pedido, itens), Times.Once);
        _mockMessageBus.Verify(x => x.PublicarPedidoCriadoAsync(It.IsAny<PedidoCriadoEvent>()), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync($"pedido:{pedido.Id}"), Times.Once);
    }

    [Fact]
    public async Task CriarPedidoAsync_ComPedidoNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        Pedido? pedido = null;
        var itens = new List<ItemPedido> { new() { Produto = "Produto 1", Quantidade = 1, PrecoUnitario = 10.00m } };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CriarPedidoAsync(pedido!, itens));
    }

    [Fact]
    public async Task CriarPedidoAsync_ComItensNulos_DeveLancarArgumentException()
    {
        // Arrange
        var pedido = Pedido.Criar(1, DateTime.Now, "Pendente");
        List<ItemPedido>? itens = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarPedidoAsync(pedido, itens!));
    }

    [Fact]
    public async Task CriarPedidoAsync_ComListaItensVazia_DeveLancarArgumentException()
    {
        // Arrange
        var pedido = Pedido.Criar(1, DateTime.Now, "Pendente");
        var itens = new List<ItemPedido>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarPedidoAsync(pedido, itens));
    }

    [Fact]
    public async Task ObterPedidoCompletoAsync_ComIdValido_DeveRetornarPedidoDoCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var pedido = new Pedido(id, 1, DateTime.Now, "Aprovado");
        var itens = new List<ItemPedido>
        {
            new() { Produto = "Produto 1", Quantidade = 1, PrecoUnitario = 10.00m }
        };

        var cacheDto = new Venice.Service.DTOs.PedidoCompletoCacheDTO
        {
            Pedido = pedido,
            Itens = itens
        };

        _mockCacheService.Setup(x => x.GetAsync<Venice.Service.DTOs.PedidoCompletoCacheDTO>($"pedido:{id}"))
            .ReturnsAsync(cacheDto);

        // Act
        var result = await _service.ObterPedidoCompletoAsync(id);

        // Assert
        result.pedido.Should().Be(pedido);
        result.itens.Should().BeEquivalentTo(itens);
        _mockRepository.Verify(x => x.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ObterPedidoCompletoAsync_ComIdValidoSemCache_DeveRetornarPedidoDoRepositorio()
    {
        // Arrange
        var id = Guid.NewGuid();
        var pedido = new Pedido(id, 1, DateTime.Now, "Aprovado");
        var itens = new List<ItemPedido>
        {
            new() { Produto = "Produto 1", Quantidade = 1, PrecoUnitario = 10.00m }
        };

        _mockCacheService.Setup(x => x.GetAsync<Venice.Service.DTOs.PedidoCompletoCacheDTO>($"pedido:{id}"))
            .ReturnsAsync((Venice.Service.DTOs.PedidoCompletoCacheDTO?)null);
        _mockRepository.Setup(x => x.ObterPorIdAsync(id)).ReturnsAsync(pedido);
        _mockRepository.Setup(x => x.ObterItensPorPedidoIdAsync(id)).ReturnsAsync(itens);
        _mockCacheService.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<Venice.Service.DTOs.PedidoCompletoCacheDTO>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ObterPedidoCompletoAsync(id);

        // Assert
        result.pedido.Should().Be(pedido);
        result.itens.Should().BeEquivalentTo(itens);
        _mockRepository.Verify(x => x.ObterPorIdAsync(id), Times.Once);
        _mockRepository.Verify(x => x.ObterItensPorPedidoIdAsync(id), Times.Once);
        _mockCacheService.Verify(x => x.SetAsync($"pedido:{id}", It.IsAny<Venice.Service.DTOs.PedidoCompletoCacheDTO>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task ObterPedidoCompletoAsync_ComIdInexistente_DeveLancarKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockCacheService.Setup(x => x.GetAsync<Venice.Service.DTOs.PedidoCompletoCacheDTO>($"pedido:{id}"))
            .ReturnsAsync((Venice.Service.DTOs.PedidoCompletoCacheDTO?)null);
        _mockRepository.Setup(x => x.ObterPorIdAsync(id)).ReturnsAsync((Pedido?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObterPedidoCompletoAsync(id));
    }
} 