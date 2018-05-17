using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class PickupPersistentData : PersistentData
    {
        private bool isPickedUp;

        public PickupPersistentData(string uniqueID) : base(uniqueID)
        {
            IsPickedUp = false;
        }

        public bool IsPickedUp
        {
            get { return isPickedUp; }
            set { isPickedUp = value; }
        }
    }
} // end of namespace