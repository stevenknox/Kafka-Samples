using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Kafka.Public;
using Kafka.Public.Loggers;
using System.Text;

namespace SimpleHostedService
{
    class Program
    {
        static void Main(string[] args)
        {
           CreateHostBuilder(args).Build().Run();
        }

        //Register the consumer first so its ready and listening when the producer kicks off
        private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, collection) => {
                collection.AddHostedService<ConsumerHostedService>();
                collection.AddHostedService<ProducerHostedService>();
            });
    }

    //Our consumer is using the Kafka-Sharp lib as an alternative to the official Confluence.Kafka lib
    //purely for learning expi
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> logger;
        private ClusterClient _cluster;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger)
        {
            this.logger = logger;
            _cluster = new ClusterClient(new Configuration {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        } 

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest(topic: "SimpleHostedService");
            _cluster.MessageReceived += record => 
            {
                logger.LogInformation($"Receieved {Encoding.UTF8.GetString(record.Value as byte[])}");
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }

    //Our producer is using the official Confluence.Kafka lib
    public class ProducerHostedService : IHostedService
    {
        private readonly ILogger<ProducerHostedService> logger;
        private IProducer<Null, string> _producer;

        public ProducerHostedService(ILogger<ProducerHostedService> logger)
        {
            this.logger = logger;
            var config = new ProducerConfig {
                BootstrapServers = "localhost:9092"
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        } 
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                var value =$"Hello World {i}";
               logger.LogInformation(value);
               await _producer.ProduceAsync(topic: "SimpleHostedService", new Message<Null, string>()
               {
                   Value = value
               }, cancellationToken);
            }

            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           _producer?.Dispose();
           return Task.CompletedTask;
        }
    }
}
