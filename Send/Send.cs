using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.UserName = "quangphat";
            factory.Password = "number8";
            using (var conn = factory.CreateConnection())
            {
                //HelloWord(conn, args);
                //EmitLog(conn, args);
                EmitLogExchangeDirect(conn, args);
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void EmitLogExchangeDirect(IConnection conn, string[] args)
        {
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                channel.ExchangeDeclare(exchange: "direct_logs2", type: ExchangeType.Topic);

                var severity = (args.Length > 0) ? args[0] : "info";
                var message = (args.Length > 1)
                              ? string.Join(" ", args.Skip(1).ToArray())
                              : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: severity=="error"? "direct_logs": "direct_logs2",
                    routingKey: severity,
                    basicProperties: null,
                    body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
            }
        }

        private static void EmitLog(IConnection conn, string[] args)
        {
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs",type: ExchangeType.Fanout);

                string message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs",
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                Console.WriteLine($"[x] Sent {message}");
            }
        }

        private static void HelloWord(IConnection conn, string[] args)
        {
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                string message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();

                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);
                Console.WriteLine($"[x] Sent {message}");
            }
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "hello world");
        }
    }
}
