// Script de inicialização do MongoDB para Venice.Orders
// Este script é executado automaticamente quando o container MongoDB é iniciado pela primeira vez

// Conectar ao banco de dados
db = db.getSiblingDB('venice_orders');

// Criar coleções se não existirem
db.createCollection('pedidos');
db.createCollection('itens_pedido');

// Criar índices para melhor performance
db.pedidos.createIndex({ "clienteId": 1 });
db.pedidos.createIndex({ "data": 1 });
db.pedidos.createIndex({ "status": 1 });
db.pedidos.createIndex({ "id": 1 }, { unique: true });

db.itens_pedido.createIndex({ "pedidoId": 1 });
db.itens_pedido.createIndex({ "id": 1 }, { unique: true });

// Inserir dados de exemplo (opcional)
db.pedidos.insertOne({
    id: "550e8400-e29b-41d4-a716-446655440000",
    clienteId: 1,
    data: new Date("2024-01-15T10:30:00Z"),
    status: "Aprovado",
    valorTotal: 45.50,
    createdAt: new Date(),
    updatedAt: new Date()
});

db.itens_pedido.insertMany([
    {
        id: "550e8400-e29b-41d4-a716-446655440001",
        pedidoId: "550e8400-e29b-41d4-a716-446655440000",
        produto: "Produto Exemplo 1",
        quantidade: 2,
        precoUnitario: 15.25,
        createdAt: new Date(),
        updatedAt: new Date()
    },
    {
        id: "550e8400-e29b-41d4-a716-446655440002",
        pedidoId: "550e8400-e29b-41d4-a716-446655440000",
        produto: "Produto Exemplo 2",
        quantidade: 1,
        precoUnitario: 15.00,
        createdAt: new Date(),
        updatedAt: new Date()
    }
]);

print("✅ Banco de dados Venice.Orders inicializado com sucesso!");
print("📊 Coleções criadas: pedidos, itens_pedido");
print("🔍 Índices criados para otimização de consultas");
print("📝 Dados de exemplo inseridos"); 