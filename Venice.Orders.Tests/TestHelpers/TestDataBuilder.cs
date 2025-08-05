using Venice.Application.DTOs;
using Venice.Domain.Entities;

namespace Venice.Orders.Tests.TestHelpers;

public static class TestDataBuilder
{
    public static PedidoRequestDTO CreateValidPedidoRequest(int clienteId = 1)
    {
        return new PedidoRequestDTO
        {
            ClienteId = clienteId,
            Data = DateTime.Now,
            Status = "Pendente",
            Itens = new List<ItemPedidoRequestDTO>
            {
                new() { Produto = "Produto Teste 1", Quantidade = 2, PrecoUnitario = 10.50m },
                new() { Produto = "Produto Teste 2", Quantidade = 1, PrecoUnitario = 25.00m }
            }
        };
    }

    public static PedidoRequestDTO CreateInvalidPedidoRequest()
    {
        return new PedidoRequestDTO
        {
            ClienteId = 1,
            Data = DateTime.Now,
            Status = "Pendente",
            Itens = new List<ItemPedidoRequestDTO>() // Lista vazia
        };
    }

    public static Pedido CreateValidPedido(Guid? id = null)
    {
        return new Pedido(id ?? Guid.NewGuid(), 1, DateTime.Now, "Pendente");
    }

    public static List<ItemPedido> CreateValidItens(Guid pedidoId)
    {
        return new List<ItemPedido>
        {
            new() { PedidoId = pedidoId, Produto = "Produto 1", Quantidade = 2, PrecoUnitario = 10.50m },
            new() { PedidoId = pedidoId, Produto = "Produto 2", Quantidade = 1, PrecoUnitario = 25.00m }
        };
    }

    public static PedidoResponseDTO CreateValidPedidoResponse(Guid id)
    {
        return new PedidoResponseDTO
        {
            Id = id,
            ClienteId = 1,
            Data = DateTime.Now,
            Status = "Aprovado",
            Itens = new List<ItemPedidoRequestDTO>
            {
                new() { Produto = "Produto 1", Quantidade = 1, PrecoUnitario = 10.00m }
            }
        };
    }

    public static List<PedidoRequestDTO> CreateMultiplePedidoRequests(int count)
    {
        var requests = new List<PedidoRequestDTO>();
        
        for (int i = 0; i < count; i++)
        {
            requests.Add(new PedidoRequestDTO
            {
                ClienteId = i + 1,
                Data = DateTime.Now.AddDays(i),
                Status = "Pendente",
                Itens = new List<ItemPedidoRequestDTO>
                {
                    new() { Produto = $"Produto {i + 1}", Quantidade = 1, PrecoUnitario = 10.00m + i }
                }
            });
        }

        return requests;
    }
} 