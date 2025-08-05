# 🐳 Venice.Orders - Docker Setup Completo

## ✅ Arquivos Criados

### 1. **docker-compose.yml** - Configuração Principal
- **API Venice.Orders** (porta 5000/5001)
- **SQL Server** (porta 1433)
- **MongoDB** (porta 27017)
- **Redis** (porta 6379)
- **RabbitMQ** (porta 5672/15672)
- **MongoDB Express** (porta 8081) - Interface web opcional

### 2. **Dockerfile** - Build da API
- Multi-stage build otimizado
- .NET 8.0
- Usuário não-root para segurança
- Build e publish automático

### 3. **docker-compose.prod.yml** - Configuração de Produção
- Variáveis de ambiente seguras
- Health checks para todos os serviços
- Limites de recursos (CPU/Memory)
- Dependências com condições de saúde

### 4. **Scripts de Automação**

#### **start-services.ps1** - Gerenciamento de Desenvolvimento
```powershell
# Comandos disponíveis:
.\start-services.ps1 -Action up -Detached    # Iniciar serviços
.\start-services.ps1 -Action down            # Parar serviços
.\start-services.ps1 -Action status          # Verificar status
.\start-services.ps1 -Action logs            # Ver logs
.\start-services.ps1 -Action clean           # Limpar tudo
```

#### **deploy-prod.ps1** - Deploy em Produção
```powershell
# Comandos disponíveis:
.\deploy-prod.ps1 -Setup                     # Configurar ambiente
.\deploy-prod.ps1 -Deploy                    # Fazer deploy
.\deploy-prod.ps1 -Backup                    # Fazer backup
.\deploy-prod.ps1 -Restore -BackupPath path  # Restaurar backup
```

### 5. **Arquivos de Configuração**

#### **mongo-init.js** - Inicialização do MongoDB
- Criação de coleções
- Índices para performance
- Dados de exemplo

#### **env.example** - Variáveis de Ambiente
- Template para configuração segura
- Senhas fortes para produção

#### **.dockerignore** - Otimização de Build
- Exclusão de arquivos desnecessários
- Build mais rápido e imagem menor

### 6. **Documentação**

#### **README-Docker.md** - Guia Completo
- Instruções detalhadas
- Troubleshooting
- Configuração avançada
- Segurança

#### **test-api.http** - Testes da API
- Exemplos de requisições
- Compatível com REST Client (VS Code)

## 🚀 Como Usar

### Desenvolvimento
```powershell
# 1. Iniciar todos os serviços
.\start-services.ps1 -Action up -Detached

# 2. Verificar status
.\start-services.ps1 -Action status

# 3. Testar API
# Acesse: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Produção
```powershell
# 1. Configurar variáveis de ambiente
copy env.example .env
# Editar .env com senhas seguras

# 2. Setup inicial
.\deploy-prod.ps1 -Setup

# 3. Deploy
.\deploy-prod.ps1 -Deploy

# 4. Backup (opcional)
.\deploy-prod.ps1 -Backup
```

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

## 🔧 Recursos Avançados

### Health Checks
- Todos os serviços incluem health checks
- Verificação automática de disponibilidade
- Logs de status de saúde

### Volumes Persistentes
- `sqlserver_data`: Dados do SQL Server
- `mongodb_data`: Dados do MongoDB
- `redis_data`: Dados do Redis
- `rabbitmq_data`: Dados do RabbitMQ

### Rede Docker
- Rede isolada `venice-network`
- Comunicação segura entre containers
- Subnet configurada: 172.20.0.0/16

### Segurança
- Usuário não-root na API
- Senhas configuráveis
- Health checks para monitoramento
- Volumes isolados

## 🧪 Testes

### Testes da API
```bash
# Criar pedido
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

# Obter pedido
curl -X GET "http://localhost:5000/api/Pedidos/{id}"
```

### Testes com Arquivo HTTP
- Use o arquivo `test-api.http` com REST Client
- Exemplos prontos para testar
- Cenários de erro incluídos

## 📊 Monitoramento

### Interfaces Web
- **MongoDB Express**: http://localhost:8081
- **RabbitMQ Management**: http://localhost:15672

### Logs
```powershell
# Logs de todos os serviços
docker-compose logs -f

# Logs específicos
docker-compose logs venice-orders-api
docker-compose logs mongodb
docker-compose logs rabbitmq
```

## 🔒 Segurança em Produção

### Checklist
- [ ] Alterar todas as senhas padrão
- [ ] Configurar variáveis de ambiente
- [ ] Usar HTTPS em produção
- [ ] Configurar firewall
- [ ] Implementar autenticação
- [ ] Configurar backup automático
- [ ] Monitoramento de logs
- [ ] Alertas de saúde dos serviços

### Variáveis de Ambiente Obrigatórias
```bash
SQL_PASSWORD=SuaSenhaForte123!
MONGO_USER=venice_admin
MONGO_PASSWORD=SuaSenhaMongo456!
REDIS_PASSWORD=SuaSenhaRedis789!
RABBITMQ_USER=venice_user
RABBITMQ_PASSWORD=SuaSenhaRabbit012!
```

## 🚀 Próximos Passos

1. **Teste o setup**: Execute `.\start-services.ps1 -Action up -Detached`
2. **Verifique os serviços**: Acesse as URLs listadas acima
3. **Teste a API**: Use o arquivo `test-api.http`
4. **Configure produção**: Use `env.example` como base
5. **Deploy em produção**: Execute `.\deploy-prod.ps1 -Setup -Deploy`

## 📝 Notas Importantes

- **Primeira execução**: Pode demorar alguns minutos para baixar as imagens
- **Recursos**: Mínimo 8GB RAM recomendado
- **Portas**: Verifique se as portas não estão em uso
- **Firewall**: Configure exceções se necessário
- **Backup**: Configure backup regular dos volumes

---

**✅ Setup Docker Completo para Venice.Orders** 🚀

*Todos os serviços necessários estão configurados e prontos para uso!* 