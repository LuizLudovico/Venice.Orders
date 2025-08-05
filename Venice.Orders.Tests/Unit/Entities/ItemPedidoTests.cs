using FluentAssertions;
using Venice.Domain.Entities;
using Xunit;

namespace Venice.Orders.Tests.Unit.Entities;

public class ItemPedidoTests
{
    [Fact]
    public void ItemPedido_ComPropriedadesValidas_DeveSerCriadoCorretamente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var produto = "Produto Teste";
        var quantidade = 2;
        var precoUnitario = 10.50m;

        // Act
        var itemPedido = new ItemPedido
        {
            PedidoId = pedidoId,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        };

        // Assert
        itemPedido.Should().NotBeNull();
        itemPedido.PedidoId.Should().Be(pedidoId);
        itemPedido.Produto.Should().Be(produto);
        itemPedido.Quantidade.Should().Be(quantidade);
        itemPedido.PrecoUnitario.Should().Be(precoUnitario);
    }

    [Fact]
    public void ItemPedido_ComQuantidadeZero_DeveSerValido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var produto = "Produto Teste";
        var quantidade = 0;
        var precoUnitario = 10.50m;

        // Act
        var itemPedido = new ItemPedido
        {
            PedidoId = pedidoId,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        };

        // Assert
        itemPedido.Quantidade.Should().Be(0);
    }

    [Fact]
    public void ItemPedido_ComPrecoUnitarioZero_DeveSerValido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var produto = "Produto Teste";
        var quantidade = 1;
        var precoUnitario = 0m;

        // Act
        var itemPedido = new ItemPedido
        {
            PedidoId = pedidoId,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        };

        // Assert
        itemPedido.PrecoUnitario.Should().Be(0);
    }
} 