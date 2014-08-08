using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IGrain1 : Orleans.IGrain
    {
        Task<string> SayHello();
    }
}
