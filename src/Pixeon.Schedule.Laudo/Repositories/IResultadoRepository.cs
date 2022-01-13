using Pixeon.Schedule.Laudo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixeon.Schedule.Laudo.Repositories
{
    public interface IResultadoRepository
    {
        Task<IList<Resultado>> BuscarResultado();
    }
}
