# Venice.Orders - Docker Setup

Este documento explica como executar a aplicação Venice.Orders usando Docker Compose.

## 🐳 Pré-requisitos

- **Docker Desktop** instalado e rodando
- **Docker Compose** (incluído no Docker Desktop)
- **8GB+ de RAM** disponível para os containers
- **10GB+ de espaço em disco** para volumes

## 🚀 Início Rápido

### 1. Iniciar todos os serviços
```powershell
# Usando o script PowerShell (recomendado)
.\start-services.ps1 -Action up -Detached

# Ou usando Docker Compose diretamente
docker-compose up -d
```

### 2. Verificar status dos serviços
```powershell
.\start-services.ps1 -Action status
```

### 3. Acessar a aplicação
- **API**: http://localhost:5000
- **Swagger/OpenAPI**: http://localhost:5000/swagger

## 📋 Serviços Disponíveis

| Serviço | Porta | URL | Credenciais |
|---------|-------|-----|-------------|
| **API Venice.Orders** | 5000 | http://localhost:5000 | - |
| **API HTTPS** | 5001 | https://localhost:5001 | - |
| **SQL Server** | 1433 | localhost:1433 | sa / YourStrong@Passw0rd |
| **MongoDB** | 27017 | localhost:27017 | admin / password |
| **MongoDB Express** | 8081 | http://localhost:8081 | admin / password |
| **Redis** | 6379 | localhost:6379 | - |
| **RabbitMQ** | 5672 | localhost:5672 | admin / password |
| **RabbitMQ Management** | 15672 | http://localhost:15672 | admin / password |

## 🛠️ Comandos Úteis

### Script PowerShell
```powershell
# Iniciar serviços em background
.\start-services.ps1 -Action up -Detached

# Iniciar com rebuild
.\start-services.ps1 -Action up -Build -Detached

# Parar serviços
.\start-services.ps1 -Action down

# Ver logs
.\start-services.ps1 -Action logs

# Verificar status
.\start-services.ps1 -Action status

# Limpar tudo
.\start-services.ps1 -Action clean
```

### Docker Compose Direto
```bash
# Iniciar serviços
docker-compose up -d

# Reconstruir imagens
docker-compose build --no-cache

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

## 🧪 Testando a API

### 1. Criar um pedido
```bash
curl -X POST "http://localhost:5000/api/Pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": 1,
    "data": "2024-01-15T10:30:00Z",
    "status": "Pendente",
    "itens": [
      {
        "produto": "Produto Teste",
        "quantidade": 2,
        "precoUnitario": 15.50
      }
    ]
  }'
```

### 2. Obter um pedido
```bash
curl -X GET "http://localhost:5000/api/Pedidos/{id-do-pedido}"
```

## 🔧 Configuração Avançada

### Variáveis de Ambiente
As seguintes variáveis podem ser modificadas no `docker-compose.yml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__MongoDB=mongodb://mongodb:27017/venice_orders
  - ConnectionStrings__RabbitMQ=amqp://admin:password@rabbitmq:5672
  - ConnectionStrings__Redis=redis:6379
  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=VeniceOrders;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
```

### Volumes Persistentes
Os dados são persistidos nos seguintes volumes:
- `sqlserver_data`: Dados do SQL Server
- `mongodb_data`: Dados do MongoDB
- `redis_data`: Dados do Redis
- `rabbitmq_data`: Dados do RabbitMQ

### Health Checks
Todos os serviços incluem health checks para garantir que estejam funcionando corretamente.

## 🐛 Troubleshooting

### Problemas Comuns

#### 1. Porta já em uso
```bash
# Verificar portas em uso
netstat -ano | findstr :5000
netstat -ano | findstr :1433
netstat -ano | findstr :27017

# Parar processo que está usando a porta
taskkill /PID <PID> /F
```

#### 2. Containers não iniciam
```bash
# Verificar logs
docker-compose logs

# Verificar status dos containers
docker-compose ps

# Reconstruir imagens
docker-compose build --no-cache
```

#### 3. Problemas de conectividade
```bash
# Verificar rede Docker
docker network ls
docker network inspect venice-network

# Reiniciar containers
docker-compose restart
```

#### 4. Limpeza completa
```bash
# Parar e remover tudo
docker-compose down -v --remove-orphans
docker system prune -f
docker volume prune -f
```

### Logs Específicos
```bash
# Logs da API
docker-compose logs venice-orders-api

# Logs do MongoDB
docker-compose logs mongodb

# Logs do RabbitMQ
docker-compose logs rabbitmq
```

## 📊 Monitoramento

### MongoDB Express
- **URL**: http://localhost:8081
- **Usuário**: admin
- **Senha**: password

### RabbitMQ Management
- **URL**: http://localhost:15672
- **Usuário**: admin
- **Senha**: password

## 🔒 Segurança

### Credenciais Padrão
⚠️ **IMPORTANTE**: As credenciais padrão são apenas para desenvolvimento. Para produção:

1. Altere todas as senhas no `docker-compose.yml`
2. Use variáveis de ambiente para credenciais
3. Configure SSL/TLS para conexões
4. Implemente autenticação adequada

### Exemplo de Configuração Segura
```yaml
environment:
  - SA_PASSWORD=${SQL_PASSWORD}
  - MONGO_INITDB_ROOT_PASSWORD=${MONGO_PASSWORD}
  - RABBITMQ_DEFAULT_PASS=${RABBITMQ_PASSWORD}
```

## 🚀 Deploy em Produção

### 1. Configurar variáveis de ambiente
```bash
export SQL_PASSWORD="SuaSenhaForte123!"
export MONGO_PASSWORD="SuaSenhaMongo456!"
export RABBITMQ_PASSWORD="SuaSenhaRabbit789!"
```

### 2. Usar docker-compose.prod.yml
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### 3. Configurar backup dos volumes
```bash
# Backup do MongoDB
docker run --rm -v venice-network_mongodb_data:/data -v $(pwd):/backup mongo:7.0 tar czf /backup/mongodb-backup.tar.gz /data

# Backup do SQL Server
docker run --rm -v venice-network_sqlserver_data:/var/opt/mssql -v $(pwd):/backup mcr.microsoft.com/mssql/server:2022-latest tar czf /backup/sqlserver-backup.tar.gz /var/opt/mssql
```

## 📝 Notas Importantes

1. **Primeira execução**: O MongoDB será inicializado com dados de exemplo
2. **Performance**: Os containers podem demorar alguns minutos para iniciar completamente
3. **Recursos**: Certifique-se de ter RAM suficiente (mínimo 8GB recomendado)
4. **Firewall**: Verifique se as portas não estão bloqueadas pelo firewall
5. **Antivírus**: Alguns antivírus podem interferir com o Docker

## 🤝 Suporte

Se encontrar problemas:

1. Verifique os logs: `docker-compose logs`
2. Consulte a documentação do Docker
3. Verifique se todos os pré-requisitos estão atendidos
4. Tente uma limpeza completa e reinicie

---

**Desenvolvido para o desafio Venice.Orders** 🚀 