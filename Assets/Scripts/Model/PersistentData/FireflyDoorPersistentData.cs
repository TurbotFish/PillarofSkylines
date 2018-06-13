using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class FireflyDoorPersistentData : PersistentData
    {
        public int CollectedFirefliesCount { get; set; }
        public bool DoorOpened { get; set; }

        public FireflyDoorPersistentData(string unique_id) : base(unique_id)
        {

        }
    }
} // end of namespace