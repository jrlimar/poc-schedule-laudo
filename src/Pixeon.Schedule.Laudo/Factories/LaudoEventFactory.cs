using Pixeon.Schedule.Laudo.Domain;
using Pixeon.Schedule.Laudo.Events;
using System.Collections.Generic;

namespace Pixeon.Schedule.Laudo.Factories
{
    public static class LaudoEventFactory
    {
        public static IEnumerable<AddLaudoEvent> CreateLaudoIntegration(IList<Resultado> resultados)
        {
            foreach (var resultado in resultados)
            {
                var integration = new AddLaudoEvent(resultado.Valor);

                yield return integration;
            }
        }
    }
}
