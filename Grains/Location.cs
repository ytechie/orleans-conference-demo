using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    [StorageProvider(ProviderName = "AzureStorage")]
    public class Location : GrainBase<ILocationState>, ILocation
    {
        public Task SetName(string name)
        {
            State.Name = name;
            return TaskDone.Done;
        }

        public Task<string> GetName()
        {
            return Task.FromResult(State.Name);
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
