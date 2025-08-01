namespace Venice.Domain.Interfaces.Bus;

public interface IMessageBus
{
    Task PublicarPedidoCriadoAsync(object mensagem);
}
