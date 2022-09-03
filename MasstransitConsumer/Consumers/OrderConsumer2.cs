using MassTransit;
using ShareModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MasstransitConsumer.Consumers
{
    public class OrderConsumer2 : IConsumer<Order>
    {
        public OrderConsumer2()
        {

        }
        public async Task Consume(ConsumeContext<Order> context)
        {
            var x = context.Message;
            Console.WriteLine("Consume from order consumer2: " + x.Name);
        }
        //public async Task Consume(ConsumeContext<OrderCreate> context)
        //{
        //    var x = context.Message;
        //    Console.WriteLine(x.OrderName);
        //}
    }
}
