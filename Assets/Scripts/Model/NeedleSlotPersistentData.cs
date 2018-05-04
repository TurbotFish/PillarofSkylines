using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class NeedleSlotPersistentData : PersistentData
    {
        private bool containsNeedle;

        public NeedleSlotPersistentData(string uniqueId, bool containsNeedle) : base(uniqueId)
        {
            this.containsNeedle = containsNeedle;
        }

        public bool ContainsNeedle
        {
            get { return containsNeedle; }
            set { containsNeedle = value; }
        }
    }
}