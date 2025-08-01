using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Venice.Domain.Interfaces.Bus;

namespace Venice.Service.Bus;

public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQMessageBus()
    {
        Inicializar();
    }

    private void Inicializar()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost", // use "rabbitmq" se estiver usando Docker
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "pedido_criado",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public Task PublicarPedidoCriadoAsync(object mensagem)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));

        _channel.BasicPublish(
            exchange: "",
            routingKey: "pedido_criado",
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
