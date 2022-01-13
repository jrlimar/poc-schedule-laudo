using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pixeon.Api.AppSettings;
using Pixeon.Api.Handlers;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;

namespace Pixeon.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //ngnix
            services.Configure<ForwardedHeadersOptions>(x => x.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

            ConfigureAppSettings(services);
            ConfigureSwagger(services);
            ConfigureRebus(services);

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //nginx
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.AddSingleton<RabbitSettings>(Configuration.GetSection("RabbitMq").Get<RabbitSettings>());
        }

        private void ConfigureRebus(IServiceCollection services)
        {
            var rabbit = Configuration.GetSection("RabbitMq").Get<RabbitSettings>(); 
            var uriRabbit = $"amqp://{rabbit.UserName}:{rabbit.Password}@{rabbit.HostName}:{rabbit.Port}";
            var nomeFila = "Laudo-Rebus-2";

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
                        o.SimpleRetryStrategy(errorQueueAddress: "Laudo-Erro-Retry", maxDeliveryAttempts: 2, secondLevelRetriesEnabled: true);
                        o.SetBusName("Poc Rebus2");
                    })
                );

            services.AutoRegisterHandlersFromAssemblyOf<LaudoEventHandler>();
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Poc",
                    Version = "v1",
                    Description = "Poc",
                    License = new OpenApiLicense { Name = "PIXEON", Url = new Uri("https://www.pixeon.com") }

                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
    }
}
