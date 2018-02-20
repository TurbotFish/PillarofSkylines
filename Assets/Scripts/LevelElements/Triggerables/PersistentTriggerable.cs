using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    [Serializable]
    public class PersistentTriggerable
    {
        //###########################################################

        [SerializeField]
        private string id;

        [SerializeField]
        private bool triggered;

        //###########################################################

        public string Id { get { return id; } }

        public bool Triggered { get { return triggered; } set { triggered = value; } }

        //###########################################################

        public PersistentTriggerable(TriggerableObject triggerable)
        {
            id = triggerable.Id;
            triggered = triggerable.Triggered;
        }

        //###########################################################
    }
} //end of namespace