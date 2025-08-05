# Multi-stage build para otimizar o tamanho da imagem
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["Venice.Orders.API/Venice.Application.csproj", "Venice.Orders.API/"]
COPY ["Venice.Orders.Service/Venice.Service.csproj", "Venice.Orders.Service/"]
COPY ["Venice.Orders.Domain/Venice.Domain.csproj", "Venice.Orders.Domain/"]
COPY ["Venice.Orders.Data/Venice.Data.csproj", "Venice.Orders.Data/"]
COPY ["Venice.Orders.CrossCutting/Venice.CrossCutting.csproj", "Venice.Orders.CrossCutting/"]

# Restaurar dependências
RUN dotnet restore "Venice.Orders.API/Venice.Application.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
RUN dotnet build "Venice.Orders.API/Venice.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Venice.Orders.API/Venice.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Criar usuário não-root para segurança
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Venice.Application.dll"]
