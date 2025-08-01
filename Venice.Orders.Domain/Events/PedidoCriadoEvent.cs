namespace Venice.Domain.Events;

public class PedidoCriadoEvent
{
    public Guid PedidoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime Data { get; set; }

    public PedidoCriadoEvent(Guid pedidoId, int clienteId, DateTime data)
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Data = data;
    }
}
