using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.LevelElements
{
    [CustomEditor(typeof(AirReceptor))]
    public class AirReceptorInspector : TriggerInspector
    {
        private AirReceptor airReceptor;

        protected override void OnEnable()
        {
            base.OnEnable();

            airReceptor = target as AirReceptor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
} //end of namespace