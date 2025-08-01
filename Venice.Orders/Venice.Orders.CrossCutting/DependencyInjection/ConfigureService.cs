using Microsoft.Extensions.DependencyInjection;
using Venice.Domain.Interfaces.Bus;
using Venice.Domain.Interfaces.Services;
using Venice.Service.Bus;
using Venice.Service.Services;

namespace CrossCutting.DependencyInjection
{
    public static class ConfigureService
    {
        public static void ConfigureDependenciesService(IServiceCollection services)
        {
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddSingleton<IMessageBus, RabbitMQMessageBus>();
        }
    }
}
