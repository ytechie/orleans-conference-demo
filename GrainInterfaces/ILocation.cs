using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ILocation : IGrain
    {
        Task<string> Name { get; }

        Task SetName(string name);
    }
}
