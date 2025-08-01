using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Venice.Application.DTOs;
using Venice.Domain.Entities;
using Venice.Domain.Interfaces.Services;

namespace Venice.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IDistributedCache _cache;

    public PedidosController(IPedidoService pedidoService, IDistributedCache cache)
    {
        _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequestDTO dto)
    {
        if (dto == null || dto.Itens == null || !dto.Itens.Any())
            return BadRequest("Pedido inválido ou sem itens.");

        var pedido = Pedido.Criar(dto.ClienteId, dto.Data, dto.Status);

        var itens = dto.Itens.Select(i => new ItemPedido
        {
            Produto = i.Produto,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario            
        }).ToList();

        await _pedidoService.CriarPedidoAsync(pedido, itens);

        // TODO: Publicar no RabbitMQ no futuro

        return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedido.Id }, new { pedido.Id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPedidoPorId(Guid id)
    {
        string cacheKey = $"pedido:{id}";

        try
        {
            string? cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var fromCache = JsonSerializer.Deserialize<PedidoResponseDTO>(cached);
                return Ok(fromCache);
            }
        }
        catch (Exception ex)
        {
            // Logar erro de cache (não deve impedir consulta ao banco)
            Console.WriteLine($"Erro ao acessar o cache: {ex.Message}");
        }

        var (pedido, itens) = await _pedidoService.ObterPedidoCompletoAsync(id);

        if (pedido == null)
            return NotFound();

        var response = new PedidoResponseDTO
        {
            Id = pedido.Id,
            ClienteId = pedido.ClienteId,
            Data = pedido.Data,
            Status = pedido.Status,
            Itens = itens.Select(i => new ItemPedidoRequestDTO
            {
                Produto = i.Produto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };

        try
        {
            var serialized = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gravar no cache: {ex.Message}");
        }

        return Ok(response);
    }
}
