using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;

namespace GrainInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IGrain1
    /// </summary>
    public interface IGrain1 : Orleans.IGrain
    {
        Task<string> SayHello();
    }
}
