using Dapper;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using System.Data;
using Xunit;
using static Questao5.Domain.Enumerators.Enuns;

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
        public async Task Handle_ValidRequest_ShouldReturnIdMovimento()
        {
            // Arrange
            var request = new CreateMovimentoRequest
            {
                IdRequisicao = Guid.NewGuid(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                TipoMovimento = TipoMovimento.Credito,
                Valor = 100
            };

            _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>()).Returns(new ContaCorrente
            {
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                Numero = 123,
                Nome = "Katherine Sanchez",
                Ativo = StatusConta.Nao
            });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
    }
}
