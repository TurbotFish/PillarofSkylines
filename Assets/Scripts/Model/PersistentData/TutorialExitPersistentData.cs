using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class TutorialExitPersistentData : PersistentData
    {
        public bool IsOpen { get; set; }

        public TutorialExitPersistentData(string unique_id) : base(unique_id)
        {

        }
    }
} // end of namespace