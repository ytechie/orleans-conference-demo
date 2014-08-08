using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using log4net;

namespace Grains
{
    public class Grain1 : Orleans.GrainBase, IGrain1
    {
        public Task<string> SayHello()
        {
            return Task.FromResult("Hello World!");
        }
    }
}
