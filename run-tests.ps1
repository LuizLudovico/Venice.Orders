# Script para executar testes do projeto Venice.Orders
param(
    [string]$TestType = "all",
    [switch]$Coverage,
    [switch]$Verbose
)

Write-Host "=== Executando Testes Venice.Orders ===" -ForegroundColor Green

# Verificar se o .NET está instalado
try {
    $dotnetVersion = dotnet --version
    Write-Host "Usando .NET $dotnetVersion" -ForegroundColor Yellow
} catch {
    Write-Host "Erro: .NET SDK não encontrado. Instale o .NET 8.0 SDK." -ForegroundColor Red
    exit 1
}

# Verificar se o Docker está disponível (para testes funcionais)
$dockerAvailable = $false
try {
    docker --version | Out-Null
    $dockerAvailable = $true
    Write-Host "Docker disponível para testes funcionais" -ForegroundColor Yellow
} catch {
    Write-Host "Docker não encontrado. Testes funcionais podem falhar." -ForegroundColor Yellow
}

# Função para executar testes
function Run-Tests {
    param(
        [string]$Filter,
        [string]$Description
    )
    
    Write-Host "`n=== Executando $Description ===" -ForegroundColor Cyan
    
    $testCommand = "dotnet test"
    
    if ($Filter -ne "all") {
        $testCommand += " --filter `"Category=$Filter`""
    }
    
    if ($Coverage) {
        $testCommand += " --collect:`"XPlat Code Coverage`""
    }
    
    if ($Verbose) {
        $testCommand += " --verbosity normal"
    } else {
        $testCommand += " --verbosity minimal"
    }
    
    Write-Host "Comando: $testCommand" -ForegroundColor Gray
    
    $result = Invoke-Expression $testCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ $Description executados com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "❌ $Description falharam!" -ForegroundColor Red
        return $false
    }
    
    return $true
}

# Executar testes baseado no tipo especificado
$allSuccess = $true

switch ($TestType.ToLower()) {
    "unit" {
        $allSuccess = Run-Tests "Unit" "Testes Unitários"
    }
    "integration" {
        $allSuccess = Run-Tests "Integration" "Testes de Integração"
    }
    "functional" {
        if (-not $dockerAvailable) {
            Write-Host "⚠️  Docker não disponível. Testes funcionais podem falhar." -ForegroundColor Yellow
        }
        $allSuccess = Run-Tests "Functional" "Testes Funcionais"
    }
    "performance" {
        $allSuccess = Run-Tests "Performance" "Testes de Performance"
    }
    "all" {
        Write-Host "`n=== Executando TODOS os testes ===" -ForegroundColor Magenta
        
        $unitSuccess = Run-Tests "Unit" "Testes Unitários"
        $integrationSuccess = Run-Tests "Integration" "Testes de Integração"
        
        if ($dockerAvailable) {
            $functionalSuccess = Run-Tests "Functional" "Testes Funcionais"
        } else {
            Write-Host "⚠️  Pulando testes funcionais (Docker não disponível)" -ForegroundColor Yellow
            $functionalSuccess = $true
        }
        
        $performanceSuccess = Run-Tests "Performance" "Testes de Performance"
        
        $allSuccess = $unitSuccess -and $integrationSuccess -and $functionalSuccess -and $performanceSuccess
    }
    default {
        Write-Host "Tipo de teste inválido: $TestType" -ForegroundColor Red
        Write-Host "Tipos válidos: unit, integration, functional, performance, all" -ForegroundColor Yellow
        exit 1
    }
}

# Resultado final
Write-Host "`n=== Resumo dos Testes ===" -ForegroundColor Green

if ($allSuccess) {
    Write-Host "🎉 Todos os testes executados com sucesso!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "💥 Alguns testes falharam!" -ForegroundColor Red
    exit 1
} 