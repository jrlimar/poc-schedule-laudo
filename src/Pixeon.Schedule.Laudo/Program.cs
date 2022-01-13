using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pixeon.Schedule.Laudo.AppSettings;
using Pixeon.Schedule.Laudo.Handlers;
using Pixeon.Schedule.Laudo.Repositories;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;

namespace Pixeon.Schedule.Laudo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<LaudoJob>();
                    services.AddSingleton<IResultadoRepository, ResultadoRepository>();

                    var rabbit = hostContext.Configuration.GetSection("RabbitMq").Get<RabbitSettings>();
                    var uriRabbit = $"amqp://{rabbit.UserName}:{rabbit.Password}@{rabbit.HostName}:{rabbit.Port}";
                    var nomeFila = "Laudo-Rebus-Job";

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Uri: {uriRabbit}");
                    Console.ForegroundColor = ConsoleColor.Black;

                    services.AddRebus(configure => configure
                            .Transport(t => t.UseRabbitMq(uriRabbit, nomeFila))
                            .Routing(r =>
                            {
                                r.TypeBased()
                                    .MapAssemblyOf<LaudoEventHandler>(nomeFila);
                            })
                            .Timeouts(t => t.StoreInMemory())
                            .Options(o =>
                            {
                                o.SetNumberOfWorkers(1);
                                o.SetMaxParallelism(1);
                                o.SimpleRetryStrategy(errorQueueAddress: "Laudo-Job-Erro-Retry", maxDeliveryAttempts: 2, secondLevelRetriesEnabled: true);
                                o.SetBusName("Poc Rebus");
                            })
                        );

                    services.AutoRegisterHandlersFromAssemblyOf<LaudoEventHandler>();

                });
    }
}
