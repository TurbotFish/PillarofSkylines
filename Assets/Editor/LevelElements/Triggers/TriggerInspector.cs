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
        private Trigger trigger;
        private SerializedProperty idProperty;

        protected virtual void OnEnable()
        {
            trigger = target as Trigger;
            idProperty = serializedObject.FindProperty("id");
            //string oldId = idProperty.stringValue;
            //bool idChanged = false;

            ////set Id if not set
            //if (string.IsNullOrEmpty(idProperty.stringValue))
            //{
            //    idProperty.stringValue = Guid.NewGuid().ToString();
            //    idChanged = true;
            //}

            ////check for duplicate id
            //var triggerIds = new List<string>();
            //foreach(var trigger in FindObjectsOfType<Trigger>())
            //{
            //    triggerIds.Add(trigger.Id);
            //}

            //while (triggerIds.Contains(idProperty.stringValue))
            //{
            //    idProperty.stringValue = Guid.NewGuid().ToString();
            //    idChanged = true;
            //}

            ////apply modifications
            //if (idChanged)
            //{
            //    serializedObject.ApplyModifiedPropertiesWithoutUndo();
            //    self.OnIdChanged(oldId);
            //}
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Id", idProperty.stringValue);
        }
    }
} //end of namespace