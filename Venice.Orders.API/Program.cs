using CrossCutting.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Venice.Data.Context;
using Venice.Domain.Interfaces.Cache;
using Venice.Domain.Interfaces.Services;
using Venice.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ====== DbContext do SQL Server ======
builder.Services.AddDbContext<PedidoSqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"))
);

// ====== MongoDb ======
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<PedidoMongoDbContext>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    var databaseName = builder.Configuration.GetSection("MongoSettings")["Database"];
    return new PedidoMongoDbContext(connectionString, databaseName);
});

// ====== Redis Cache ======
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Registro do RedisCacheService para DI
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// Registro do PedidoService
builder.Services.AddScoped<IPedidoService, PedidoService>();

// ====== Injeção de dependência do domínio e infraestrutura ======
ConfigureService.ConfigureDependenciesService(builder.Services);
ConfigureRepository.ConfigureDependenciesRepository(builder.Services);

// ====== Controllers & Swagger ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ====== Pipeline HTTP ======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
