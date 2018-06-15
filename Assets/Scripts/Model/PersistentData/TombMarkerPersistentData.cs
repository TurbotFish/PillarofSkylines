using Game.EchoSystem;
using Game.LevelElements;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class TombMarkerPersistentData : PersistentData, IWaypoint
    {
        //###########################################################

        // -- CONSTANTS

        [Header("Waypoint")]
        [SerializeField] private bool useCameraAngle;
        [SerializeField] private float cameraAngle;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsWaypoint { get; set; }
        public bool IsTombCollected { get; set; }

        private Vector3 LastPosition;
        private TombMarker ActiveInstance;

        //###########################################################

        // -- INITIALIZATION

        public TombMarkerPersistentData(string unique_id, Vector3 position) : base(unique_id)
        {
            LastPosition = position;
        }

        //###########################################################

        // -- INQUIRIES

        public TombMarker GetActiveInstance()
        {
            return ActiveInstance;
        }

        public bool UseCameraAngle { get { return useCameraAngle; } }
        public float CameraAngle { get { return cameraAngle; } }

        public Vector3 Position
        {
            get
            {
                if (ActiveInstance != null)
                {
                    return ActiveInstance.Transform.position;
                }
                return LastPosition;
            }
        }

        //###########################################################

        // -- OPERATIONS

        void IWaypoint.OnWaypointRemoved()
        {
            if (IsWaypoint)
            {
                IsWaypoint = false;
                ActiveInstance?.ActivateWaypoint(false);
            }
        }

        public void SetActiveInstance(TombMarker instance = null)
        {
            if (instance == null && ActiveInstance != null)
            {
                LastPosition = ActiveInstance.Transform.position;
            }

            ActiveInstance = instance;
        }
    }
} // end of namespace