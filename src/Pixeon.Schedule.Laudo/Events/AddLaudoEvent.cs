using System;

namespace Pixeon.Schedule.Laudo.Events
{
    public class AddLaudoEvent
    {
        public AddLaudoEvent(decimal resultado)
        {
            Id = Guid.NewGuid();
            Resultado = resultado;
        }

        public Guid Id { get; private set; }
        public decimal Resultado { get; private set; }

    }
}
