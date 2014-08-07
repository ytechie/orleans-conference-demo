using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public interface IAttendeeState : IGrainState
    {
        string Name { get; set; }
        Dictionary<DateTime, ILocation> Locations { get; set; }
    }
}
