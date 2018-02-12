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

        //###########################################################

        public string Id { get { return id; } }

        //###########################################################

        public PersistentTriggerable(TriggerableObject triggerable)
        {
            //id = triggerable
        }

        //###########################################################
    }
} //end of namespace