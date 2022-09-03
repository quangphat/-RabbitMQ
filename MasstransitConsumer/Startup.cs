
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MasstransitConsumer.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace MasstransitConsumer
{
    public class Startup
    {
        public Startup()
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<OrderConsumer>();
            services.AddSingleton<OrderCreateConsumer>();
            services.AddSingleton<IHostedService, BusService>();
            services.AddSingleton(u =>
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost"), host =>
                    {
                        host.Username("quangphat");
                        host.Password("number8");
                    });

                    //RegisterFanoutExchange(cfg);
                    
                    RegisterDirectExchange(cfg);
                });
            });
        }

        private void RegisterFanoutExchange(IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.ReceiveEndpoint("get-order", ep =>
            {
                ep.PrefetchCount = 16;
                ep.Consumer<OrderConsumer>();
            });
            cfg.ReceiveEndpoint("get-order2", ep =>
            {
                ep.PrefetchCount = 16;
                ep.Consumer<OrderConsumer2>();
            });

            cfg.ReceiveEndpoint("create-order", ep =>
            {
                ep.PrefetchCount = 16;
                ep.Consumer<OrderCreateConsumer>();
            });
        }


        private void RegisterDirectExchange(IRabbitMqBusFactoryConfigurator cfg)
        {
            //cfg.ReceiveEndpoint("order-queue", e =>
            //{
            //    e.Durable = true;
            //    e.ConfigureConsumeTopology = false;
            //    e.Consumer<OrderCreateConsumer>();
            //    e.Bind("order-exchange", p =>
            //    {
            //        p.ExchangeType = ExchangeType.Direct;
            //        p.RoutingKey = "order-key";
            //    });
            //});

            cfg.ReceiveEndpoint("order-create-queue", e =>
            {
                //e.ConfigureConsumeTopology = false;
                e.Consumer<OrderCreateConsumer>();
                e.Consumer<OrderTikiCreateConsumer>();
                e.Bind("order-exchange", p =>
                {
                    p.ExchangeType = ExchangeType.Direct;
                    p.RoutingKey = "order-create-key";
                    p.RoutingKey = "tikiorder-create-key";
                });
            });

            //cfg.ReceiveEndpoint("order-create-queue", e =>
            //{
            //    e.ConfigureConsumeTopology = false;
            //    e.Consumer<OrderTikiCreateConsumer>();
            //    e.Bind("order-exchange", p =>
            //    {
            //        p.ExchangeType = ExchangeType.Direct;
            //        p.RoutingKey = "tikiorder-create-key";
            //    });
            //});
        }
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {

        }
    }

    public class BusService : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
