using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.EchoSystem
{
    /// <summary>
    /// Interfaces used by level elements that can be used as a waypoint.
    /// </summary>
    public interface IWaypoint
    {
        string UniqueId { get; }
        Vector3 Position { get; }
        float CameraAngle { get; }
        bool UseCameraAngle { get; }

        void OnWaypointRemoved();
    }
} // end of namespace