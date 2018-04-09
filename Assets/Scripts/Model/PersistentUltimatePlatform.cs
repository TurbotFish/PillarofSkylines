using System.Collections;
using System.Collections.Generic;
using Game.LevelElements;
using UnityEngine;

namespace Game.Model
{
    public class PersistentUltimatePlatform : PersistentTriggerable
    {
        public UltimatePlatform Master { get; set; }
        public bool MasterSetOnce { get; set; }

        public Vector3 LocalPosition { get; set; }
        public int CurrentPoint { get; set; }
        public int NextPoint { get; set; }
        public eUltimatePlatformState CurrentState { get; set; }
        public eUltimatePlatformState PreviousState { get; set; }
        public bool GoingForward { get; set; }
        public bool FinishingMovement { get; set; }
        public float Elapsed { get; set; }

        public PersistentUltimatePlatform(string uniqueId, bool triggered) : base(uniqueId, triggered)
        {
        }
    }
} //end of namespace