namespace Questao1
{
    public class ContaBancaria
    {
        public int NumeroConta { get; set; }
        public string Titular { get; set; }
        private double Saldo { get; set; }

        private const double TaxaSaque = 3.50;

        public ContaBancaria(int numeroConta, string titular)
        {
            NumeroConta = numeroConta;
            Titular = titular;
            Saldo = 0.0;
        }

        public ContaBancaria(int numeroConta, string titular, double depositoInicial) : this(numeroConta, titular)
        {
            Depositar(depositoInicial);
        }

        public void Depositar(double valor)
        {
            Saldo += valor;
        }

        public void Sacar(double valor)
        {
            Saldo -= valor + TaxaSaque;
        }

        public override string ToString()
        {
            return $"Conta {NumeroConta}, Titular: {Titular}, Saldo: $ {Saldo:F2}";
        }
    }
}
