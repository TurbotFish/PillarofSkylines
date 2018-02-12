using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.LevelElements
{
    [CustomEditor(typeof(RacingTrigger))]
    public class RecingTriggerInspector : TriggerInspector
    {
        private RacingTrigger self;

        protected override void OnEnable()
        {
            base.OnEnable();

            self = target as RacingTrigger;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
} //end of namespace