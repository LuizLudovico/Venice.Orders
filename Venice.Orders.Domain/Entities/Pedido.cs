using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Venice.Domain.Entities;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public int ClienteId { get; private set; }
    public DateTime Data { get; private set; }
    public string Status { get; private set; }   

    public Pedido(Guid id, int clienteId, DateTime data, string status)
    {
        Id = id;
        ClienteId = clienteId;
        Data = data;
        Status = status;
    }

    public static Pedido Criar(int clienteId, DateTime data, string status)
    {
        return new Pedido(Guid.NewGuid(), clienteId, data, status);
    }    
}
