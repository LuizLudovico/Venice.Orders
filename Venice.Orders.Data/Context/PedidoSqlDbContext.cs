using Microsoft.EntityFrameworkCore;
using Venice.Domain.Entities;

namespace Venice.Data.Context
{
    public class PedidoSqlDbContext(DbContextOptions<PedidoSqlDbContext> options) : DbContext(options)
    {
        public DbSet<Pedido> Pedidos { get; set; } = null!;
    }
}
