using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class Location : GrainBase, ILocation
    {
        public Task<string> Name { get; private set; }

        public Task SetName(string name)
        {
            Name = Task.FromResult(name);
            return TaskDone.Done;
        }
    }
}
