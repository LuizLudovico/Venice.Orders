using CrossCutting.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Venice.Data.Context;
using Venice.Domain.Interfaces.Bus;
using Venice.Domain.Interfaces.Cache;
using Venice.Domain.Interfaces.Services;
using Venice.Service.Bus;
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
    return new PedidoMongoDbContext(connectionString!, databaseName!);
});

// ====== Redis ======
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// ====== Message Bus ======
builder.Services.AddScoped<IMessageBus, RabbitMQMessageBus>();

// ====== Services ======
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// ====== Repository ======
ConfigureRepository.ConfigureDependenciesRepository(builder.Services);

// ====== Controllers ======
builder.Services.AddControllers();

// ====== Swagger ======
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ====== Configure the HTTP request pipeline ======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
