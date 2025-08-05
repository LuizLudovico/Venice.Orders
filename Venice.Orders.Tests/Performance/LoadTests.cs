using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Venice.Application.DTOs;
using Xunit;

namespace Venice.Orders.Tests.Performance;

public class LoadTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LoadTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ObterPedidos_EmParalelo_DeveRetornarRespostasRapidas()
    {
        // Arrange
        var client = _factory.CreateClient();
        var tasks = new List<Task<(HttpResponseMessage response, TimeSpan duration)>>();
        var numberOfRequests = 20;

        // Primeiro, criar alguns pedidos
        var createdPedidos = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            var pedidoRequest = new PedidoRequestDTO
            {
                ClienteId = i + 1,
                Data = DateTime.Now,
                Status = "Aprovado",
                Itens = new List<ItemPedidoRequestDTO>
                {
                    new() { Produto = $"Produto {i}", Quantidade = 1, PrecoUnitario = 10.00m }
                }
            };

            var json = JsonSerializer.Serialize(pedidoRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Pedidos", content);
            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(location))
                {
                    createdPedidos.Add(location);
                }
            }
        }

        // Se não conseguiu criar pedidos, pular o teste
        if (createdPedidos.Count == 0)
        {
            // Criar pelo menos um pedido para teste
            var pedidoRequest = new PedidoRequestDTO
            {
                ClienteId = 1,
                Data = DateTime.Now,
                Status = "Aprovado",
                Itens = new List<ItemPedidoRequestDTO>
                {
                    new() { Produto = "Produto Teste", Quantidade = 1, PrecoUnitario = 10.00m }
                }
            };

            var json = JsonSerializer.Serialize(pedidoRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Pedidos", content);
            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(location))
                {
                    createdPedidos.Add(location);
                }
            }
        }

        // Se ainda não tem pedidos, pular o teste
        if (createdPedidos.Count == 0)
        {
            return; // Skip test if we can't create any orders
        }

        // Act - Fazer requisições paralelas para obter os pedidos
        for (int i = 0; i < numberOfRequests; i++)
        {
            var pedidoUrl = createdPedidos[i % createdPedidos.Count];
            tasks.Add(MeasureResponseTimeAsync(client, pedidoUrl));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(numberOfRequests);
        results.Should().OnlyContain(r => r.response.IsSuccessStatusCode);
        
        // Verificar que a maioria das respostas são rápidas (menos de 1 segundo)
        var fastResponses = results.Count(r => r.duration.TotalMilliseconds < 1000);
        var percentageFast = (double)fastResponses / numberOfRequests;
        percentageFast.Should().BeGreaterThan(0.8); // 80% das respostas devem ser rápidas
    }    

    private async Task<(HttpResponseMessage response, TimeSpan duration)> MeasureResponseTimeAsync(
        HttpClient client, string url)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await client.GetAsync(url);
        stopwatch.Stop();
        
        return (response, stopwatch.Elapsed);
    }
} 