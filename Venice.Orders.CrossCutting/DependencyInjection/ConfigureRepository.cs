using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Venice.Data.Repository;
using Venice.Domain.Interfaces.Repository;

namespace CrossCutting.DependencyInjection
{
    public static class ConfigureRepository
    {
        public static void ConfigureDependenciesRepository(IServiceCollection services)
        {
            // Repositório
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            // MongoDB
            services.AddSingleton<IMongoClient>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });

            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var databaseName = configuration["MongoSettings:Database"];
                return client.GetDatabase(databaseName);
            });
        }
    }
}
