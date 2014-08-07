using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace EventProcessor
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            string eventHubName = ConfigurationManager.AppSettings["eventHubName"];
            string consumerGroupName = ConfigurationManager.AppSettings["consumerGroupName"];

            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("EventProcessor entry point called");

            string connectionString = GetServiceBusConnectionString();

            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            EventHubDescription ehd = namespaceManager.GetEventHub(eventHubName);
            namespaceManager.CreateConsumerGroupIfNotExistsAsync(ehd.Path, consumerGroupName);

            Receiver r = new Receiver(eventHubName, connectionString);
            r.MessageProcessingWithPartitionDistribution();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working");
            }
        }

        private static string GetServiceBusConnectionString()
        {
            string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.TransportType = Microsoft.ServiceBus.Messaging.TransportType.Amqp;
            return builder.ToString();
        }


        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
