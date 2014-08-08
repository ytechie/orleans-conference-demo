using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using log4net;
using Orleans;

namespace Grains
{
    //We're speicifying a storage provider from our config
    //You can build your own storage provider if you like
    [StorageProvider(ProviderName = "AzureStorage")]
    public class Attendee : GrainBase<IAttendeeState>, IAttendee
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Task SetName(string name)
        {
            State.Name = name;

            //There is a helper for methods that would normally return void
            return TaskDone.Done;
        }

        public Task<string> GetName()
        {
            return Task.FromResult(State.Name);
        }

        public async Task CheckIn(ILocation location)
        {
            if (State.Locations.Count > 50)
            {
                var oldestCheckin = State.Locations.OrderBy(l => l.Key).Take(1).Single().Key;
                State.Locations.Remove(oldestCheckin);
            }

            State.Locations.Add(DateTime.UtcNow, location);
            var locName = await location.GetName();

            Log.DebugFormat("{0}, welcome to the session '{1}'", State.Name, locName);

            //Persist state
            await State.WriteStateAsync();
        }

        public async Task<ILocation> GetCurrentLocation()
        {
            var checkIn = State.Locations.OrderByDescending(x => x.Key).FirstOrDefault();

            if (checkIn.Value == null)
                return null;

            return checkIn.Value;
        }

        public override async Task ActivateAsync()
        {
            await base.ActivateAsync();

            //Give the grain a default name for display purposes
            if (string.IsNullOrEmpty(State.Name))
            {
                State.Name = this.GetPrimaryKeyLong().ToString();
            }
        }
    }
}
