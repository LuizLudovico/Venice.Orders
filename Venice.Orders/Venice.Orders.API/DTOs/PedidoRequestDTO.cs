namespace Venice.Application.DTOs;

public class PedidoRequestDTO
{
    public int ClienteId { get; set; }
    public DateTime Data { get; set; }
    public string Status { get; set; } = null!;
    public List<ItemPedidoRequestDTO> Itens { get; set; } = new();
}
