using Dapper;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Application.Handlers
{
    public class GetSaldoHandler : IRequestHandler<GetSaldoRequest, SaldoResponse>
    {
        private readonly IDbConnection _dbConnection;

        public GetSaldoHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<SaldoResponse> Handle(GetSaldoRequest request, CancellationToken cancellationToken)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>("SELECT * FROM contacorrente WHERE idcontacorrente = @Id", new { Id = request.IdContaCorrente });

            if (conta == null)
                throw new Exception("INVALID_ACCOUNT");

            if (conta.Ativo == Domain.Enumerators.Enuns.StatusConta.Nao)
                throw new Exception("INACTIVE_ACCOUNT");

            var creditos = await _dbConnection.QueryFirstOrDefaultAsync<decimal>("SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'C'", new { Id = request.IdContaCorrente });
            var debitos = await _dbConnection.QueryFirstOrDefaultAsync<decimal>("SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'D'", new { Id = request.IdContaCorrente });

            var saldo = creditos - debitos;

            return new SaldoResponse
            {
                Numero = conta.Numero,
                Nome = conta.Nome,
                DataHora = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
}
