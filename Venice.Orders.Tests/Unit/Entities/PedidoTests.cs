using FluentAssertions;
using Venice.Domain.Entities;
using Xunit;

namespace Venice.Orders.Tests.Unit.Entities;

public class PedidoTests
{
    [Fact]
    public void Criar_ComParametrosValidos_DeveCriarPedidoComIdUnico()
    {
        // Arrange
        var clienteId = 1;
        var data = DateTime.Now;
        var status = "Pendente";

        // Act
        var pedido = Pedido.Criar(clienteId, data, status);

        // Assert
        pedido.Should().NotBeNull();
        pedido.Id.Should().NotBe(Guid.Empty);
        pedido.ClienteId.Should().Be(clienteId);
        pedido.Data.Should().Be(data);
        pedido.Status.Should().Be(status);
    }

    [Fact]
    public void Criar_ComParametrosValidos_DeveGerarIdsDiferentes()
    {
        // Arrange
        var clienteId = 1;
        var data = DateTime.Now;
        var status = "Pendente";

        // Act
        var pedido1 = Pedido.Criar(clienteId, data, status);
        var pedido2 = Pedido.Criar(clienteId, data, status);

        // Assert
        pedido1.Id.Should().NotBe(pedido2.Id);
    }

    [Fact]
    public void Constructor_ComParametrosValidos_DeveCriarPedidoCorretamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clienteId = 1;
        var data = DateTime.Now;
        var status = "Aprovado";

        // Act
        var pedido = new Pedido(id, clienteId, data, status);

        // Assert
        pedido.Id.Should().Be(id);
        pedido.ClienteId.Should().Be(clienteId);
        pedido.Data.Should().Be(data);
        pedido.Status.Should().Be(status);
    }
} 