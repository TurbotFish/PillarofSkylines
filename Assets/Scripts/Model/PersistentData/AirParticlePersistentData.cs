using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class AirParticlePersistentData : PersistentData
    {
        private bool isPicked;
        private bool hasBeenReceived;

        public AirParticlePersistentData(string uniqueId) : base(uniqueId)
        {
        }

        public bool IsPicked
        {
            get { return isPicked; }
            set { isPicked = value; }
        }

        public bool HasBeenReceived
        {
            get { return hasBeenReceived; }
            set { hasBeenReceived = value; }
        }
    }
} // end of namespace