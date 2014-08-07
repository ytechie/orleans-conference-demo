namespace EventProcessor
{
    using System.Diagnostics;
    using System.Runtime.Serialization.Json;
    using System.Threading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using EventTypes;

    public class SimpleEventProcessor : IEventProcessor
    {
        IDictionary<string, int> map;
        PartitionContext partitionContext;
        Stopwatch checkpointStopWatch;

        public SimpleEventProcessor()
        {
            this.map = new Dictionary<string, int>();
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("SimpleEventProcessor initialize.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));
            this.partitionContext = context;
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                foreach (EventData eventData in events)
                {
                    string key = eventData.PartitionKey;

                    if (eventData.Properties["Type"].ToString().Equals("TweetEvent"))
                    {
                        TweetEvent tweet = JsonConvert.DeserializeObject<TweetEvent>(Encoding.Unicode.GetString(eventData.GetBytes()));

                        Console.WriteLine(string.Format("Message received.  **Tweet**, Created By: '{1}': '{2}'",
                            this.partitionContext.Lease.PartitionId, tweet.Author, tweet.Text));

                        //TODO: process this message
                    }
                    else if (eventData.Properties["Type"].ToString().Equals("VendorCheckin"))
                    {
                        VendorCheckin checkin = JsonConvert.DeserializeObject<VendorCheckin>(Encoding.UTF8.GetString(eventData.GetBytes()));

                        Console.WriteLine(string.Format("Message received.  **VendorCheckin**, Created By: '{1}': '{2}'",
                            this.partitionContext.Lease.PartitionId, checkin.VendorID, checkin.BadgeID));

                        //TODO: process this message
                    }
                    else if (eventData.Properties["Type"].ToString().Equals("ProximitrySensor"))
                    {
                        string tmp = Encoding.Unicode.GetString(eventData.GetBytes());
                        ProximitySensorEvent proximityevent = JsonConvert.DeserializeObject<ProximitySensorEvent>(tmp);

                        Console.WriteLine(string.Format("Message received.  **ProximitrySensor**, Created By: '{1}': '{2:HH:mm}'",
                            this.partitionContext.Lease.PartitionId, proximityevent.SensorID, proximityevent.TransmissionTime));

                        //TODO: process this message
                    }
                }

                //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
                if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(1))
                {
                    //await context.CheckpointAsync();
                    lock (this)
                    {
                        this.checkpointStopWatch.Reset();
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error in processing: " + exp.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("Processor Shuting Down.  Partition '{0}', Reason: '{1}'.", this.partitionContext.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                //await context.CheckpointAsync();
            }
        }

        object DeserializeEventData(EventData eventData)
        {
            if (eventData.Properties["Type"].ToString().Equals("TweetEvent"))
                return JsonConvert.DeserializeObject<TweetEvent>(Encoding.Unicode.GetString(eventData.GetBytes()));

            return null; // default if we couldn't map the type... 
        }
    }
}
