using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    public interface IWaypoint
    {
        string UniqueId { get; }
        Vector3 Position { get; }

        void OnWaypointRemoved();
    }
} // end of namespace