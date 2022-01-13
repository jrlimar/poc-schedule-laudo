namespace Pixeon.Api.Events
{
    public class AddLaudoEvent
    {
        public string Nome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public AddLaudoEvent(string nome, int quantidade, decimal valorUnitario)
        {
            Nome = nome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }
    }
}
