using Dapper;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Application.Handlers
{
    public class CreateMovimentoHandler : IRequestHandler<CreateMovimentoRequest, string>
    {
        private readonly IDbConnection _dbConnection;

        public CreateMovimentoHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> Handle(CreateMovimentoRequest request, CancellationToken cancellationToken)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @Id", new { Id = Guid.Parse(request.IdContaCorrente) });

            if (conta == null)
                throw new Exception("INVALID_ACCOUNT");

            if (conta.Ativo == Domain.Enumerators.Enuns.StatusConta.Nao)
                throw new Exception("INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE");

            if (request.TipoMovimento != Domain.Enumerators.Enuns.TipoMovimento.Credito && request.TipoMovimento != Domain.Enumerators.Enuns.TipoMovimento.Debito)
                throw new Exception("INVALID_TYPE");

            var idempotencia = await _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(
                "SELECT * FROM idempotencia WHERE chave_idempotencia = @IdRequisicao",
                new { IdRequisicao = request.IdRequisicao });

            if (idempotencia != null)
            {
                return idempotencia.Resultado;
            }

            var idMovimento = Guid.NewGuid().ToString();
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new
                {
                    IdMovimento = idMovimento,
                    IdContaCorrente = Guid.Parse(request.IdContaCorrente),
                    DataMovimento = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                    request.TipoMovimento,
                    request.Valor
                });

            
            await _dbConnection.ExecuteAsync(
                "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@IdRequisicao, @Requisicao, @Resultado)",
                new
                {
                    IdRequisicao = request.IdRequisicao,
                    Requisicao = request.ToString(),
                    Resultado = idMovimento
                });

            return idMovimento;
        }
    }
}
