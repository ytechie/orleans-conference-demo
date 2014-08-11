using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTypes;
using GrainInterfaces;
using Grains;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Orleans;

namespace Ingestor
{
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

                        var tweetGrain = TweetFactory.GetGrain(tweet.GetHashCode());
                        tweetGrain.SetTweetInfo(tweet.Text);

                        //Console.WriteLine(string.Format("Message received.  **Tweet**, Created By: '{1}': '{2}'",
                        //    this.partitionContext.Lease.PartitionId, tweet.Author, tweet.Text));

                        //TODO: process this message
                    }
                    else if (eventData.Properties["Type"].ToString().Equals("VendorCheckin"))
                    {
                        VendorCheckin checkin = JsonConvert.DeserializeObject<VendorCheckin>(Encoding.UTF8.GetString(eventData.GetBytes()));

                        //Console.WriteLine(string.Format("Message received.  **VendorCheckin**, Created By: '{1}': '{2}'",
                        //    this.partitionContext.Lease.PartitionId, checkin.VendorID, checkin.BadgeID));

                        int vendorId;
                        int badgeId;
                        if (int.TryParse(checkin.VendorID, out vendorId) && int.TryParse(checkin.BadgeID, out badgeId))
                        {
                            var location = LocationFactory.GetGrain(vendorId);
                            var attendee = AttendeeFactory.GetGrain(badgeId);

                            await attendee.CheckIn(location);
                        }
                    }
                    else if (eventData.Properties["Type"].ToString().Equals("ProximitrySensor"))
                    {
                        var tmp = Encoding.Unicode.GetString(eventData.GetBytes());
                        var proximityevent = JsonConvert.DeserializeObject<ProximitySensorEvent>(tmp);

                        //Console.WriteLine(string.Format("Message received.  **ProximitrySensor**, Created By: '{1}': '{2:HH:mm}'",
                        //    this.partitionContext.Lease.PartitionId, proximityevent.SensorID, proximityevent.TransmissionTime));

                        var sensorId = Math.Abs(proximityevent.SensorID.GetHashCode());
                        var sensor = ProximitySensorFactory.GetGrain(sensorId);
                        await sensor.ReadingReceived(proximityevent);
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
