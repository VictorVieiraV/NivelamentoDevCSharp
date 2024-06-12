using MediatR;
using static Questao5.Domain.Enumerators.Enuns;

namespace Questao5.Application.Commands.Requests
{
    public class CreateMovimentoRequest : IRequest<string>
    {
        public Guid IdRequisicao { get; set; }
        public string IdContaCorrente { get; set; }
        public TipoMovimento TipoMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
