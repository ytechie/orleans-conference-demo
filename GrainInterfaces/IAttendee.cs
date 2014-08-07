using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IAttendee : IGrain
    {
        //List<ILocation> LocationHistory { get; }
        Task SetName(string name);

        Task<string> GetName();

        Task CheckIn(ILocation location);

        Task<ILocation> GetCurrentLocation();
    }
}
