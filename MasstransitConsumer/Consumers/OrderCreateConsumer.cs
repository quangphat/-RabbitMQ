using MassTransit;
using ShareModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MasstransitConsumer.Consumers
{
    public class OrderCreateConsumer : IConsumer<OrderCreate>
    {
        public OrderCreateConsumer()
        {

        }
        public async Task Consume(ConsumeContext<OrderCreate> context)
        {
            var x = context.Message;
            Console.WriteLine(x.OrderName);
        }
    }

    public class OrderTikiCreateConsumer : IConsumer<OrderTiki>
    {
        public OrderTikiCreateConsumer()
        {

        }
        public async Task Consume(ConsumeContext<OrderTiki> context)
        {
            var x = context.Message;
            Console.WriteLine(x.OrderName);
        }
    }
}
