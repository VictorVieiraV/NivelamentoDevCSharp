using Dapper;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using System.Data;
using Xunit;

namespace Questao5.Tests.Handlers
{
    public class CreateMovimentoHandlerTests
    {
        private readonly IDbConnection _dbConnection;
        private readonly CreateMovimentoHandler _handler;

        public CreateMovimentoHandlerTests()
        {
            _dbConnection = Substitute.For<IDbConnection>();
            _handler = new CreateMovimentoHandler(_dbConnection);
        }

        [Fact]
        public async Task Handle_ContaInvalida_DeveLancarExcecao()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = "C",
                Valor = 100
            };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult((ContaCorrente)null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT", exception.Message);
        }

        [Fact]
        public async Task Handle_ContaInativa_DeveLancarExcecao()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = "C",
                Valor = 100
            };

            var conta = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = Domain.Enumerators.Enuns.StatusConta.Nao };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("INACTIVE_ACCOUNT", exception.Message);
        }

        [Fact]
        public async Task Handle_ValorInvalido_DeveLancarExcecao()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = "C",
                Valor = -100
            };

            var conta = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = Domain.Enumerators.Enuns.StatusConta.Sim };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_VALUE", exception.Message);
        }

        [Fact]
        public async Task Handle_TipoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = "X",
                Valor = 100
            };

            var conta = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = Domain.Enumerators.Enuns.StatusConta.Sim };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_TYPE", exception.Message);
        }

        [Fact]
        public async Task Handle_ChaveIdempotenciaExistente_DeveRetornarResultadoArmazenado()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = "C",
                Valor = 100
            };

            var conta = new ContaCorrente { IdContaCorrente = Guid.NewGuid().ToString(), Ativo = Domain.Enumerators.Enuns.StatusConta.Sim };
            var idempotencia = new Idempotencia { ChaveIdempotencia = request.IdRequisicao, Resultado = "existing-result" };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(idempotencia));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("existing-result", result);
        }
    }
}