using Orleans;

namespace Grains
{
    public interface ILocationState : IGrainState
    {
        string Name { get; set; }
    }
}
