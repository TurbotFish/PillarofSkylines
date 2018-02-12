using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    [Serializable]
    public class PersistentTrigger
    {
        //###########################################################

        [SerializeField]
        private string id;

        [SerializeField]
        private bool triggerState;

        //###########################################################

        public string Id { get { return id; } }

        public bool TriggerState { get { return triggerState; } set { triggerState = value; } }

        //###########################################################

        public PersistentTrigger(Trigger trigger)
        {
            id = trigger.Id;
            triggerState = trigger.TriggerState;
        }

        //###########################################################
    }
} //end of namespace