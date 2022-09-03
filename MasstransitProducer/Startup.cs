using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ShareModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitProducer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IBus>(u =>
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost"), host =>
                    {
                        host.Username("quangphat");
                        host.Password("number8");
                    });

                    RegisterDirectExchange(cfg);
                });
            });
        }

        private void RegisterDirectExchange(IRabbitMqBusFactoryConfigurator cfg)
        {
            //cfg.Send<Order>(x => x.UseRoutingKeyFormatter(conntext => "order-key"));
            ////cfg.Message<Order>(p => p.SetEntityName("order-exchange"));
            //cfg.Publish<Order>(p => p.ExchangeType = ExchangeType.Fanout);

            cfg.Send<OrderCreate>(x => x.UseRoutingKeyFormatter(conntext => "order-create-key"));
            cfg.Message<OrderCreate>(p => p.SetEntityName("order-exchange"));
            cfg.Publish<OrderCreate>(p => p.ExchangeType = ExchangeType.Direct);

            cfg.Send<OrderTiki>(x => x.UseRoutingKeyFormatter(conntext => "tikiorder-create-key"));
            cfg.Message<OrderTiki>(p => p.SetEntityName("order-exchange"));
            cfg.Publish<OrderTiki>(p => p.ExchangeType = ExchangeType.Direct);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
