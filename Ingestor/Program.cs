using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Ingestor
{
    class Program
    {
        static void Main(string[] args)
        {
            //Give our orleans cluster time to start
            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));

            Console.WriteLine("Loading Orleans Client...");
            Orleans.OrleansClient.Initialize("DevTestClientConfiguration.xml");
            Console.WriteLine("Orleans Client Loaded...");


            var eventHubName = ConfigurationManager.AppSettings["eventHubName"];
            var consumerGroupName = ConfigurationManager.AppSettings["consumerGroupName"];

            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("EventProcessor entry point called");

            var connectionString = GetServiceBusConnectionString();

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var ehd = namespaceManager.GetEventHub(eventHubName);
            namespaceManager.CreateConsumerGroupIfNotExistsAsync(ehd.Path, consumerGroupName);

            var r = new Receiver(eventHubName, connectionString);
            r.MessageProcessingWithPartitionDistribution();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working");
            }
        }

        private static string GetServiceBusConnectionString()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            var builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.TransportType = TransportType.Amqp;
            return builder.ToString();
        }
    }
}
