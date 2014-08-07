using System;
using System.Linq;
using System.Threading.Tasks;
using EventTypes;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class ProximitySensor : GrainBase, IProximitySensor
    {
        public async Task ReadingReceived(ProximitySensorEvent evt)
        {
            var sensorId = Math.Abs(evt.SensorID.GetHashCode());
            foreach (var badge in evt.FoundBadges.Where(b => b != null))
            {
                var attendee = AttendeeFactory.GetGrain(badge.BadgeID);
                var currentLocation = await attendee.GetCurrentLocation();
                if (currentLocation.GetPrimaryKeyLong() != sensorId)
                {
                    var location = LocationFactory.GetGrain(sensorId);

                    Console.WriteLine("Checking {0} into {1}", attendee.GetPrimaryKeyLong(),
                        location.GetPrimaryKeyLong());

                    await attendee.CheckIn(location);
                }
            }
        }
    }
}
