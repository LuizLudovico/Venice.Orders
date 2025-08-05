using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text;
using Venice.Domain.Interfaces.Services;
using Xunit;

namespace Venice.Orders.Tests.Integration.Controllers;

public class PedidosControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IPedidoService> _mockPedidoService;

    public PedidosControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockPedidoService = new Mock<IPedidoService>();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove o serviço real e adiciona o mock
                var descriptors = services.Where(d => d.ServiceType == typeof(IPedidoService)).ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton(_mockPedidoService.Object);
            });
        });
    }

    [Fact]
    public async Task CriarPedido_ComPedidoNulo_DeveRetornar400BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("null", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/Pedidos", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ObterPedidoPorId_ComIdValido_DeveRetornar200Ok()
    {
        // Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();
        var pedido = new Venice.Domain.Entities.Pedido(id, 1, DateTime.Now, "Aprovado");
        var itens = new List<Venice.Domain.Entities.ItemPedido>
        {
            new() { Produto = "Produto 1", Quantidade = 1, PrecoUnitario = 10.00m }
        };

        _mockPedidoService.Setup(x => x.ObterPedidoCompletoAsync(id))
            .ReturnsAsync((pedido, itens));

        // Act
        var response = await client.GetAsync($"/api/Pedidos/{id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ObterPedidoPorId_ComIdInexistente_DeveRetornar404NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _mockPedidoService.Setup(x => x.ObterPedidoCompletoAsync(id))
            .ThrowsAsync(new KeyNotFoundException($"Pedido com ID {id} não encontrado."));

        // Act
        var response = await client.GetAsync($"/api/Pedidos/{id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ObterPedidoPorId_ComIdInvalido_DeveRetornar400BadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var idInvalido = "id-invalido";

        // Act
        var response = await client.GetAsync($"/api/Pedidos/{idInvalido}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
} 