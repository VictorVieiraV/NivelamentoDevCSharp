using Dapper;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using System.Data;
using Xunit;

namespace Questao5.Tests.Handlers
{
    public class GetSaldoHandlerTests
    {
        private readonly IDbConnection _dbConnection;
        private readonly GetSaldoHandler _handler;

        public GetSaldoHandlerTests()
        {
            _dbConnection = Substitute.For<IDbConnection>();
            _handler = new GetSaldoHandler(_dbConnection);
        }

        [Fact]
        public async Task Handle_ContaInvalida_DeveLancarExcecao()
        {
            // Arrange
            var request = new GetSaldoRequest
            {
                IdContaCorrente = Guid.Parse("B6BAFC09-6967-ED11-A567-055DFA4A16C9")
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
            var request = new GetSaldoRequest
            {
                IdContaCorrente = Guid.Parse("B6BAFC09-6967-ED11-A567-055DFA4A16C9")
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
        public async Task Handle_ContaValida_DeveRetornarSaldoCorreto()
        {
            // Arrange
            var request = new GetSaldoRequest
            {
                IdContaCorrente = Guid.Parse("B6BAFC09-6967-ED11-A567-055DFA4A16C9")
            };

            var conta = new ContaCorrente
            {
                IdContaCorrente = request.IdContaCorrente.ToString(),
                Numero = 12345,
                Nome = "Teste",
                Ativo = Domain.Enumerators.Enuns.StatusConta.Sim
            };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            _dbConnection.QueryFirstOrDefaultAsync<decimal>(
                Arg.Any<string>(),
                Arg.Any<object>())
                .Returns(Task.FromResult(200m), Task.FromResult(50m));

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(12345, response.Numero);
            Assert.Equal("Teste", response.Nome);
            Assert.Equal(150m, response.Saldo);
        }
    }
}
