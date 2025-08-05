# Testes do Projeto Venice.Orders

Este projeto contém uma suíte completa de testes para o sistema de pedidos Venice.Orders, incluindo testes unitários, de integração, funcionais e de performance.

## Estrutura dos Testes

```
Venice.Orders.Tests/
├── Unit/                           # Testes unitários
│   ├── Entities/                   # Testes das entidades de domínio
│   │   ├── PedidoTests.cs
│   │   └── ItemPedidoTests.cs
│   └── Services/                   # Testes dos serviços
│       └── PedidoServiceTests.cs
├── Integration/                    # Testes de integração
│   └── Controllers/               # Testes dos controllers
│       └── PedidosControllerTests.cs
├── Functional/                     # Testes funcionais
│   └── EndToEnd/                  # Testes end-to-end
│       └── PedidosEndToEndTests.cs
├── Performance/                    # Testes de performance
│   └── LoadTests.cs
├── TestHelpers/                    # Helpers para testes
│   └── TestDataBuilder.cs
└── README.md                      # Esta documentação
```

## Tipos de Testes

### 1. Testes Unitários (`Unit/`)

Testam componentes isolados sem dependências externas:

- **Entidades**: Testam a lógica de domínio das entidades `Pedido` e `ItemPedido`
- **Serviços**: Testam a lógica de negócio do `PedidoService` usando mocks

**Exemplo de execução:**
```bash
dotnet test --filter "Category=Unit"
```

### 2. Testes de Integração (`Integration/`)

Testam a integração entre componentes:

- **Controllers**: Testam os endpoints da API usando `WebApplicationFactory`
- **Mocks**: Usam mocks para simular dependências externas

**Exemplo de execução:**
```bash
dotnet test --filter "Category=Integration"
```

### 3. Testes Funcionais (`Functional/`)

Testam o comportamento completo do sistema:

- **End-to-End**: Testam o fluxo completo usando containers reais
- **Containers**: Usam MongoDB, RabbitMQ e Redis em containers

**Exemplo de execução:**
```bash
dotnet test --filter "Category=Functional"
```

### 4. Testes de Performance (`Performance/`)

Testam o comportamento sob carga:

- **Load Tests**: Testam múltiplas requisições simultâneas
- **Cache Tests**: Verificam a eficiência do cache

**Exemplo de execução:**
```bash
dotnet test --filter "Category=Performance"
```

## Executando os Testes

### Pré-requisitos

1. .NET 8.0 SDK
2. Docker (para testes funcionais)
3. Visual Studio 2022 ou VS Code

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes Específicos

```bash
# Apenas testes unitários
dotnet test --filter "Category=Unit"

# Apenas testes de integração
dotnet test --filter "Category=Integration"

# Apenas testes funcionais
dotnet test --filter "Category=Functional"

# Apenas testes de performance
dotnet test --filter "Category=Performance"
```

### Executar com Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar Testes em Paralelo

```bash
dotnet test --maxcpucount:4
```

## Configuração dos Testes

### Testes Funcionais

Os testes funcionais usam containers Docker para simular o ambiente real:

- **MongoDB**: Banco de dados principal
- **RabbitMQ**: Message broker
- **Redis**: Cache

Os containers são iniciados automaticamente durante os testes e limpos ao final.

### Testes de Performance

Os testes de performance verificam:

- Tempo de resposta das APIs
- Comportamento sob carga
- Eficiência do cache
- Processamento paralelo

## Padrões de Teste

### Nomenclatura

Os testes seguem o padrão: `[Metodo]_[Cenario]_[ResultadoEsperado]`

Exemplo:
```csharp
[Fact]
public async Task CriarPedido_ComDadosValidos_DeveRetornar201Created()
```

### Estrutura AAA

Todos os testes seguem o padrão Arrange-Act-Assert:

```csharp
[Fact]
public void Teste_Exemplo()
{
    // Arrange - Preparar dados
    var pedido = TestDataBuilder.CreateValidPedido();
    
    // Act - Executar ação
    var resultado = service.CriarPedido(pedido);
    
    // Assert - Verificar resultado
    resultado.Should().NotBeNull();
}
```

### TestDataBuilder

Helper para criar dados de teste consistentes:

```csharp
var pedidoRequest = TestDataBuilder.CreateValidPedidoRequest();
var pedido = TestDataBuilder.CreateValidPedido();
```

## Cobertura de Testes

Os testes cobrem:

- ✅ Entidades de domínio
- ✅ Serviços de aplicação
- ✅ Controllers da API
- ✅ Validações de entrada
- ✅ Tratamento de erros
- ✅ Cache Redis
- ✅ Message Bus (RabbitMQ)
- ✅ Performance e carga

## Troubleshooting

### Problemas Comuns

1. **Containers não iniciam**: Verifique se o Docker está rodando
2. **Testes lentos**: Os testes funcionais são mais lentos devido aos containers
3. **Timeout**: Aumente o timeout para testes de performance

### Logs

Para debug, adicione logs nos testes:

```csharp
Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
```

## Contribuindo

Ao adicionar novos testes:

1. Siga a nomenclatura estabelecida
2. Use o `TestDataBuilder` para dados consistentes
3. Adicione testes para cenários de erro
4. Mantenha os testes independentes
5. Documente testes complexos 