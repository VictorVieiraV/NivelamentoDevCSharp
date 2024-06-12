using static Questao5.Domain.Enumerators.Enuns;

namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        public string IdContaCorrente { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public StatusConta Ativo { get; set; }
    }
}
