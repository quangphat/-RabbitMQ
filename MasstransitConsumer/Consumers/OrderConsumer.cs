using MassTransit;
using ShareModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MasstransitConsumer.Consumers
{
    public class OrderConsumer : IConsumer<Order>
    {
        public OrderConsumer()
        {

        }
        public async Task Consume(ConsumeContext<Order> context)
        {
            var x = context.Message;
            Console.WriteLine(x.Name);
        }
            //public async Task Consume(ConsumeContext<OrderCreate> context)
            //{
            //    var x = context.Message;
            //    Console.WriteLine(x.OrderName);
            //}
    }
}
