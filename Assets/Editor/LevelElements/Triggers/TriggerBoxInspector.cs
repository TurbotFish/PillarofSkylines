using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.LevelElements
{
    [CustomEditor(typeof(TriggerBox))]
    public class TriggerBoxInspector : TriggerInspector
    {
        private TriggerBox self;

        protected override void OnEnable()
        {
            base.OnEnable();

            self = target as TriggerBox;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
} //end of namespace