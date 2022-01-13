using Microsoft.Extensions.Hosting;
using Pixeon.Schedule.Laudo.Factories;
using Pixeon.Schedule.Laudo.Repositories;
using Rebus.Bus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pixeon.Schedule.Laudo
{
    public class LaudoJob : BackgroundService
    {
        private readonly IBus bus;
        private readonly IResultadoRepository resultadoRepository;

        public LaudoJob(IResultadoRepository resultadoRepository, IBus bus)
        {
            this.resultadoRepository = resultadoRepository;
            this.bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Laudo Publicado! {DateTime.Now}");
                Console.ForegroundColor = ConsoleColor.Black;

                var resultados = await resultadoRepository.BuscarResultado();
                var integrations = LaudoEventFactory.CreateLaudoIntegration(resultados);

                foreach (var integration in integrations)
                {
                    await bus.Send(integration);
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
