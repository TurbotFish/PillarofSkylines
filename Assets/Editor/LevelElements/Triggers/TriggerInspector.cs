using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.LevelElements
{
    [CustomEditor(typeof(Trigger))]
    public abstract class TriggerInspector : Editor
    {
        private Trigger self;
        private SerializedProperty idProperty;

        protected virtual void OnEnable()
        {
            self = target as Trigger;
            idProperty = serializedObject.FindProperty("id");

            if (string.IsNullOrEmpty(idProperty.stringValue))
            {
                idProperty.stringValue = Guid.NewGuid().ToString();
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Id", idProperty.stringValue);
        }
    }
} //end of namespace