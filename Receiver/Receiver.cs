using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Receiver
{
    class Receiver
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.UserName = "quangphat";
            factory.Password ="number8";
            using (var conn = factory.CreateConnection())
            {
                //HelloWord(conn);
                //EmitLog(conn);
                EmitLogExchangeDirect(conn, args);
            }

            
        }

        private static void EmitLogExchangeDirect(IConnection conn, string[] args)
        {
            args= new string[3] { "info","warning", "error" };
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                channel.ExchangeDeclare(exchange: "direct_logs2", type: ExchangeType.Topic);

                var queueName = channel.QueueDeclare().QueueName;

                if(args.Length<1)
                {
                    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                                        Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }
                foreach(var severity in args)
                {
                    if(severity == "error")
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);
                    else
                        channel.QueueBind(queue: queueName, exchange: "direct_logs2", routingKey: severity);
                }
                
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey, message);
                };

                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

        private static void EmitLog(IConnection conn)
        {
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);
                };

                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

        protected static void HelloWord(IConnection conn)
        {
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"[x] reveived {message}");

                    int dots = 2;
                    int.TryParse(message.Split('.')[1], out dots);
                    Thread.Sleep(dots * 1000);
                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: "task_queue",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

       
        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            
        }
    }
}
