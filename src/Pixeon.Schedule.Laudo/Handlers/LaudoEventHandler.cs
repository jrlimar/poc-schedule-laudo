using Pixeon.Schedule.Laudo.Events;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pixeon.Schedule.Laudo.Handlers
{
    public class LaudoEventHandler : IHandleMessages<AddLaudoEvent>, IHandleMessages<IFailed<AddLaudoEvent>>
    {
        private readonly IBus bus;
        public LaudoEventHandler(IBus bus)
        {
            this.bus = bus;
        }

        public async Task Handle(AddLaudoEvent message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Laudo Em Andamento - Mensagem {typeof(AddLaudoEvent)} - {DateTime.Now}");
            Console.ForegroundColor = ConsoleColor.Black;

            Thread.Sleep(10000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Laudo Processado! - Mensagem {typeof(AddLaudoEvent)} - {DateTime.Now}");
            Console.ForegroundColor = ConsoleColor.Black;

            await Task.CompletedTask;
        }

        public async Task Handle(IFailed<AddLaudoEvent> message)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro retry depois de 10 segundos");
                Console.ForegroundColor = ConsoleColor.Black;

                await bus.DeferLocal(TimeSpan.FromSeconds(10), message.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"ERRO RETRY - {ex.Message}");

            }
        }
    }
}
