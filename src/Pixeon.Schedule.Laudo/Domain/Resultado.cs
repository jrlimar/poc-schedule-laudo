using System;

namespace Pixeon.Schedule.Laudo.Domain
{
    public class Resultado
    {
        public Resultado(decimal valor)
        {
            Id = Guid.NewGuid();
            Valor = valor;
        }

        public Guid Id { get; set; }
        public decimal Valor { get; set; }
    }
}
