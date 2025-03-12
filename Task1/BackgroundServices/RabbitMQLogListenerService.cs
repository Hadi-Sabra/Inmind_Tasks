using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Task1.Data;
using Task1.Models;

namespace Task1.Services
{
    public class RabbitMQLogListenerService : BackgroundService
    {
        private readonly ILogger<RabbitMQLogListenerService> _logger;
        private readonly TransactionDbContext _context;
        private readonly string _rabbitMqConnectionString = "your-rabbitmq-connection-string";

        public RabbitMQLogListenerService(ILogger<RabbitMQLogListenerService> logger, TransactionDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_rabbitMqConnectionString) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "logQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var jsonString = System.Text.Encoding.UTF8.GetString(body);
                    var logEntry = JsonConvert.DeserializeObject<LogEntry>(jsonString);

                    if (logEntry != null)
                    {
                        await _context.LogEntries.AddAsync(logEntry);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Log entry successfully saved from RabbitMQ.");
                    }
                };

                channel.BasicConsume(queue: "logQueue", autoAck: true, consumer: consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    }
}
