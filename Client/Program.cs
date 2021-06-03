using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Cient
{

    class Program
    {
        private static string topicName = "UserTracking";
        private static string server = "localhost:9092";
        private static Database database;
        private static SuggestionService suggestionService;
        static async Task Main(string[] args)
        {
            database = new Database();
            suggestionService = new SuggestionService(database);

            await CreateTopic();

            var events = EventGenerator.Generate(database);

            await Producer(events);

            Consumer();
        }

        private static async Task CreateTopic()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = server }).Build())
            {
                try
                {
                    //clear/delete first for this demo
                    await adminClient.DeleteTopicsAsync(new List<string> { topicName });

                    await adminClient.CreateTopicsAsync(new TopicSpecification[] { 
                        new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 1 } });
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }
        }

        private static async Task Producer(List<Event> events)
        {
            var config = new ProducerConfig { BootstrapServers = server };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<string, string>(config).Build())
            {
                foreach (var e in events)
                {
                     try
                    {
                        var dr = await p.ProduceAsync(topicName, new Message<string, string> { Key = e.User.UserId, Value= e.Product.ToString() });
                        Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    }
                    catch (ProduceException<Null, string> ex)
                    {
                        Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
                    }
                }
            }
        }

        private static void Consumer()
        {
            var conf = new ConsumerConfig
            { 
                GroupId = "test-consumer-group",
                BootstrapServers = server,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<string, string>(conf).Build())
            {
                c.Subscribe(topicName);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

                            suggestionService.Suggest(cr.Message.Key, cr.Message.Value);

                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
    }
}
