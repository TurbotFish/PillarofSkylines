using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.LevelElements
{
    [CustomEditor(typeof(LightReceptor))]
    public class LightReceptorInspector : TriggerInspector
    {
        private LightReceptor lightReceptor;

        protected override void OnEnable()
        {
            base.OnEnable();

            lightReceptor = target as LightReceptor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
} //end of namespace