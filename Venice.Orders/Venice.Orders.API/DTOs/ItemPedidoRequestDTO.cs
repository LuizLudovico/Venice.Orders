namespace Venice.Application.DTOs;

public class ItemPedidoRequestDTO
{
    public string Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
