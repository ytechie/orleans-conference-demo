using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IAttendee : IGrain
    {
        Task SetName(string name);

        Task<string> GetName();

        Task CheckIn(ILocation location);

        Task<ILocation> GetCurrentLocation();
    }
}
