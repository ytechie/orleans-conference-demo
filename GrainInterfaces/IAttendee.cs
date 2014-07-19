using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IAttendee : IGrain
    {
        Task<string> Name { get; }
        //List<ILocation> LocationHistory { get; }
        Task SetName(string name);

        Task CheckIn(ILocation location);

        Task<ILocation> GetCurrentLocation();
    }
}
