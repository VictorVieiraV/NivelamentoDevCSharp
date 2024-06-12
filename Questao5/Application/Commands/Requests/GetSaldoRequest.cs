using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    public class GetSaldoRequest : IRequest<SaldoResponse>
    {
        public Guid IdContaCorrente { get; set; }
    }
}
