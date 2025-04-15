API Bancária
Este projeto implementa uma API REST para um sistema bancário, fornecendo funcionalidades para movimentação de contas correntes e consulta de saldos. O sistema foi desenvolvido seguindo princípios de arquitetura limpa e padrões como CQRS e Mediator.
Funcionalidades
1. Movimentação de Conta Corrente

Realiza créditos e débitos em contas
Validação de regras de negócio
Suporte para idempotência (evita operações duplicadas)

2. Consulta de Saldo

Retorna o saldo atual de uma conta corrente
Inclui dados do titular e data da consulta
O saldo é calculado dinamicamente com base nas movimentações

Tecnologias Utilizadas

.NET 9: Framework mais recente da Microsoft
ASP.NET Core: Framework para desenvolvimento de APIs
Minimal API: Abordagem moderna para APIs com menos código boilerplate
SQLite: Banco de dados leve e portátil
Dapper: Micro ORM para acesso a dados
MediatR: Implementação do padrão Mediator
CQRS: Padrão de separação de responsabilidades para comandos e consultas
Swagger/OpenAPI: Documentação interativa da API

Arquitetura
O projeto segue uma arquitetura em camadas:

API: Controladores e configuração da aplicação
Application: Handlers, commands e queries
Domain: Entidades, exceções e interfaces de repositório
Infrastructure: Implementação de repositórios e acesso a dados

Estrutura do Banco de Dados
O sistema utiliza três tabelas principais:

contacorrente: Armazena dados das contas bancárias
movimento: Registra as movimentações (créditos e débitos)
idempotencia: Guarda chaves para garantir operações idempotentes

Como Executar

Pré-requisitos:

.NET 9 SDK
Um editor de código (Visual Studio, VS Code, etc.)


Clone o repositório:
git clone https://github.com/seu-usuario/api-bancaria.git
cd api-bancaria

Restaure os pacotes:
dotnet restore

Execute a aplicação:
dotnet run --project API.Bancaria

Acesse a documentação da API:
Navegue para https://localhost:7101/swagger em seu navegador

Exemplos de Uso
Consultar Saldo
GET /api/Movimentacao/{idContaCorrente}
Realizar Movimentação
POST /api/Movimentacao
json{
  "chaveIdempotencia": "8a1b9c3d-4e5f-6a7b-8c9d-0e1f2a3b4c5d",
  "idContaCorrente": "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
  "valor": 100.50,
  "tipoMovimento": "C"
}
Testes
O projeto inclui testes unitários para validar as regras de negócio e o comportamento dos handlers:
dotnet test
Principais Padrões e Conceitos

Idempotência: Implementada para garantir que uma mesma operação não seja executada múltiplas vezes
CQRS: Separação de responsabilidades entre comandos (write) e queries (read)
Domain Exceptions: Exceções de domínio para validação de regras de negócio
Repository Pattern: Abstração para acesso a dados
Dependency Injection: Injeção de dependências para acoplamento fraco entre componentes

Contribuição
Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests com melhorias para o projeto.
