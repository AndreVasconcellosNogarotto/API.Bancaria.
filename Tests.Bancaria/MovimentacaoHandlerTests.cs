﻿using API.Bancaria.Command;
using API.Bancaria.Handlers;
using API.Bancaria.Models;
using API.Bancaria.Repository;
using API.Bancaria.Response;
using Domain.Bancaria.Exceptions;
using Domain.Bancaria.Repositories;
using Moq;
using System.Text.Json;

namespace Tests.Bancaria;
public class MovimentacaoHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaCorrenteRepositoryMock;
    private readonly Mock<IMovimentoRepository> _movimentoRepositoryMock;
    private readonly Mock<IIdempotenciaRepository> _idempotenciaRepositoryMock;
    private readonly RealizarMovimentacaoHandler _handler;

    public MovimentacaoHandlerTests()
    {
        _contaCorrenteRepositoryMock = new Mock<IContaCorrenteRepository>();
        _movimentoRepositoryMock = new Mock<IMovimentoRepository>();
        _idempotenciaRepositoryMock = new Mock<IIdempotenciaRepository>();
        _handler = new RealizarMovimentacaoHandler(
            _contaCorrenteRepositoryMock.Object,
            _movimentoRepositoryMock.Object,
            _idempotenciaRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ContaInexistente_DeveLancarExcecao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "F475F943-7067-ED11-A06B-7E5DFA4A16C9",
            TipoMovimentacao = "C",
            Valor = 100.00m
        };

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ContaCorrente)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("INVALID_ACCOUNT", exception.Tipo);
    }

    [Fact]
    public async Task Handle_ContaInativa_DeveLancarExcecao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "F475F943-7067-ED11-A06B-7E5DFA4A16C9",
            TipoMovimentacao = "C",
            Valor = 100.00m
        };

        var contaInativa = new ContaCorrente
        {
            IdContaCorrente = "F475F943-7067-ED11-A06B-7E5DFA4A16C9",
            Numero = 123,
            Nome = "Ana Silva",
            Ativo = false
        };

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<string>()))
            .ReturnsAsync(contaInativa);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("INACTIVE_ACCOUNT", exception.Tipo);
    }

    [Fact]
    public async Task Handle_ValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            TipoMovimentacao = "C",
            Valor = -100.00m
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            Numero = 123,
            Nome = "João da Silva",
            Ativo = true
        };

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<string>()))
            .ReturnsAsync(contaAtiva);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("INVALID_VALUE", exception.Tipo);
    }

    [Fact]
    public async Task Handle_TipoInvalido_DeveLancarExcecao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            TipoMovimentacao = "X", // Tipo inválido
            Valor = 100.00m
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            Numero = 123,
            Nome = "João da Silva",
            Ativo = true
        };

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<string>()))
            .ReturnsAsync(contaAtiva);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("INVALID_TYPE", exception.Tipo);
    }

    [Fact]
    public async Task Handle_RequisicaoIdempotente_DeveRetornarResultadoAnterior()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            TipoMovimentacao = "C",
            Valor = 100.00m
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var expectedResponse = new MovimentacaoResponse { IdMovimentacao = "123" };
        var serializedResponse = JsonSerializer.Serialize(expectedResponse, jsonOptions);

        var idempotencia = new Idempotencia(
            "chave123",
            JsonSerializer.Serialize(command, jsonOptions),
            serializedResponse
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.IdMovimentacao);

        // Verifica que os outros repositórios não foram chamados
        _contaCorrenteRepositoryMock.Verify(x => x.ObterPorIdAsync(It.IsAny<string>()), Times.Never);
        _movimentoRepositoryMock.Verify(x => x.InserirAsync(It.IsAny<Movimentacao>()), Times.Never);
        _idempotenciaRepositoryMock.Verify(x => x.InserirAsync(It.IsAny<Idempotencia>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RequisicaoValida_DeveRealizarMovimentacao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            TipoMovimentacao = "C",
            Valor = 100.00m
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            Numero = 123,
            Nome = "João da Silva",
            Ativo = true
        };

        _idempotenciaRepositoryMock.Setup(x => x.ObterPorChaveAsync("123"))
            .ReturnsAsync((Idempotencia)null);

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync("382D323D-7067-ED11-8866-7D5DFA4A16C9"))
            .ReturnsAsync(contaAtiva);

        _movimentoRepositoryMock.Setup(x => x.InserirAsync(It.IsAny<Movimentacao>()))
            .ReturnsAsync("223");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.IdMovimentacao);

        // Verifica que os repositórios foram chamados corretamente
        _contaCorrenteRepositoryMock.Verify(x => x.ObterPorIdAsync("382D323D-7067-ED11-8866-7D5DFA4A16C9"), Times.Once);
        _movimentoRepositoryMock.Verify(x => x.InserirAsync(It.IsAny<Movimentacao>()), Times.Once);
        _idempotenciaRepositoryMock.Verify(x => x.InserirAsync(It.IsAny<Idempotencia>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DebitoValido_DevePermitirMovimentacao()
    {
        // Arrange
        var command = new RealizarMovimentacaoCommand
        {
            ChaveIdempotencia = "123",
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            TipoMovimentacao = "D", // Débito
            Valor = 100.00m
        };

        var contaAtiva = new ContaCorrente
        {
            IdContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9",
            Numero = 123,
            Nome = "João da Silva",
            Ativo = true
        };

        _idempotenciaRepositoryMock.Setup(x => x.ObterPorChaveAsync("123"))
            .ReturnsAsync((Idempotencia)null);

        _contaCorrenteRepositoryMock.Setup(x => x.ObterPorIdAsync("382D323D-7067-ED11-8866-7D5DFA4A16C9"))
            .ReturnsAsync(contaAtiva);

        _movimentoRepositoryMock.Setup(x => x.InserirAsync(It.IsAny<Movimentacao>()))
            .ReturnsAsync("123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123", result.IdMovimentacao);

        // Verifica que o movimento foi do tipo débito
        _movimentoRepositoryMock.Verify(x => x.InserirAsync(It.Is<Movimentacao>(m =>
            m.TipoMovimentacao == "D" && m.Valor == 100.00m)), Times.Once);
    }
}

