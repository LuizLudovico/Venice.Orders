using Microsoft.AspNetCore.Mvc;
using Venice.Application.DTOs;
using Venice.Domain.Entities;
using Venice.Domain.Interfaces.Services;

namespace Venice.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoRequestDTO pedidoRequest)
    {
        var pedido = Pedido.Criar(
            pedidoRequest.ClienteId,
            pedidoRequest.Data,
            pedidoRequest.Status
        );

        var itens = pedidoRequest.Itens.Select(item => new ItemPedido
        {
            Produto = item.Produto,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario
        }).ToList();

        var pedidoCriado = await _pedidoService.CriarPedidoAsync(pedido, itens);
        return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedidoCriado.Id }, pedidoCriado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedidoPorId(Guid id)
    {
        try
        {
            var (pedido, itens) = await _pedidoService.ObterPedidoCompletoAsync(id);
            return Ok(new { pedido, itens });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}


