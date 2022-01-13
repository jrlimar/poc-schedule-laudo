using Microsoft.AspNetCore.Mvc;
using Pixeon.Api.Events;
using Rebus.Bus;
using System;
using System.Threading.Tasks;

namespace Pixeon.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaudoController : Controller
    {
        private readonly IBus bus;

        public LaudoController(IBus bus)
        {
            this.bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> ReceberLaudo(string nome, int qtd, decimal valor)
        {
            var evento = new AddLaudoEvent(nome, qtd, valor);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Laudo Publicado! {DateTime.Now}");
            Console.ForegroundColor = ConsoleColor.Black;

            await bus.Send(evento);

            return Ok();
        }
    }
}
