namespace Venice.Domain.Entities;

public class ItemPedido
{
    public string Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
