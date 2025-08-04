using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Venice.Domain.Entities
{
    public class ItemPedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public Guid PedidoId { get; set; } 
        public string Produto { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
