using Game.LevelElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class TombMarkerPersistentData : PersistentData
    {
        public bool IsWaypoint { get; set; }
        public bool IsTombCollected { get; set; }

        public TombMarkerPersistentData(string unique_id) : base(unique_id)
        {

        }
    }
} // end of namespace