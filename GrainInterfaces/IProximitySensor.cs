using System.Threading.Tasks;
using EventTypes;
using Orleans;

namespace GrainInterfaces
{
    public interface IProximitySensor : IGrain
    {
        Task ReadingReceived(ProximitySensorEvent evt);
    }
}
