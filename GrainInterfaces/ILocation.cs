using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ILocation : IGrain
    {
        Task SetName(string name);

        Task<string> GetName();
    }
}
