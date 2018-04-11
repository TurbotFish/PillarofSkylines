using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public abstract class PersistentData
    {
        public string UniqueId { get; private set; }
        public event EventHandler OnPersistentDataChange;

        public PersistentData(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        protected void SendOnPersistentDataChangeEvent()
        {
            OnPersistentDataChange?.Invoke(this, EventArgs.Empty);
        }
    }
} //end of namespace