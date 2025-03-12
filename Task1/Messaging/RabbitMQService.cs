using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Task1.Models;
using Task1.Services;

namespace Task1.Messaging
{
    public class RabbitMQService
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _exchangeName;
        private readonly ITransactionLogService _transactionLogService;
        private readonly ILogger<RabbitMQService> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQService(
            IConfiguration config, 
            ITransactionLogService transactionLogService,
            ILogger<RabbitMQService> logger)
        {
            _hostname = config["RabbitMQ:HostName"] ?? "localhost";
            _username = config["RabbitMQ:UserName"] ?? "guest";
            _password = config["RabbitMQ:Password"] ?? "guest";
            _exchangeName = config["RabbitMQ:ExchangeName"] ?? "banking_exchange";
            _transactionLogService = transactionLogService;
            _logger = logger;
            
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: "topic", // Using string instead of ExchangeType.Topic
                    durable: true);

                var queueName = _channel.QueueDeclare(
                    queue: "transaction_logs_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null).QueueName;

                _channel.QueueBind(
                    queue: queueName,
                    exchange: _exchangeName,
                    routingKey: "transaction.*");

                var consumer = new EventingBasicConsumer(_channel);
                
                // Using explicit delegate to avoid ambiguity
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body, 0, body.Length);
                    
                    try
                    {
                        var transactionLog = JsonSerializer.Deserialize<TransactionLog>(message);
                        if (transactionLog != null)
                        {
                            // Using Task.Run to avoid async void
                            Task.Run(async () => 
                            {
                                await _transactionLogService.AddTransactionLogAsync(transactionLog);
                            });
                            
                            _logger.LogInformation($"Processed transaction log: {transactionLog.Id}");
                        }
                        
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing message: {ex.Message}");
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);
                
                _logger.LogInformation("RabbitMQ initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize RabbitMQ: {ex.Message}");
            }
        }

        public void PublishTransactionLog(TransactionLog transactionLog)
        {
            if (_channel == null)
            {
                _logger.LogError("Cannot publish message: RabbitMQ channel is not initialized");
                return;
            }
            
            try
            {
                var message = JsonSerializer.Serialize(transactionLog);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: $"transaction.{transactionLog.TransactionType.ToLower()}",
                    basicProperties: null,
                    body: body);
                
                _logger.LogInformation($"Published transaction log: {transactionLog.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish message: {ex.Message}");
            }
        }
    }
}