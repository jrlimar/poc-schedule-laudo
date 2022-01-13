using Pixeon.Schedule.Laudo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixeon.Schedule.Laudo.Repositories
{
    public class ResultadoRepository : IResultadoRepository
    {
        public async Task<IList<Resultado>> BuscarResultado()
        {
            var resultados = new List<Resultado>();

            resultados.Add(new Resultado(55));
            resultados.Add(new Resultado(22));
            resultados.Add(new Resultado(56));
            resultados.Add(new Resultado(89));
            resultados.Add(new Resultado(22));

            return await Task.FromResult(resultados);
        }
    }
}
