using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using log4net;
using Orleans;

namespace Grains
{
    public class Attendee : GrainBase, IAttendee
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _name;
        private Dictionary<DateTime, ILocation> _locations;

        public Task<string> Name
        {
            get
            {
                return Task.FromResult(_name);
            }
        }
        //public List<ILocation> LocationHistory { get; private set; }

        public Task SetName(string name)
        {
            _name = name;
            return TaskDone.Done;
        }

        public async Task CheckIn(ILocation location)
        {
            _locations.Add(DateTime.UtcNow, location);
            var locName = await location.Name;

            Log.DebugFormat("{0}, welcome to the session '{1}'", _name, locName);
        }

        public async Task<ILocation> GetCurrentLocation()
        {
            var checkIn = _locations.OrderByDescending(x => x.Key).FirstOrDefault();

            if (checkIn.Value == null)
                return null;

            return checkIn.Value;
        }

        public override Task ActivateAsync()
        {
            _locations = new Dictionary<DateTime, ILocation>();
            //LocationHistory = new List<ILocation>();
            return base.ActivateAsync();
        }
    }
}
