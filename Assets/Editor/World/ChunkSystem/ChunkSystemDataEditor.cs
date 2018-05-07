//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [CustomEditor(typeof(ChunkSystemData))]
//    public class ChunkSystemDataEditor : Editor
//    {
//        ChunkSystemData self;
//        SerializedProperty renderDistancesProperty;

//        internal void OnEnable()
//        {
//            self = target as ChunkSystemData;
//            renderDistancesProperty = serializedObject.FindProperty("renderDistances");
//        }

//        public override void OnInspectorGUI()
//        {
//            //
//            base.OnInspectorGUI();

//            //
//            EditorGUILayout.LabelField("Render Distances");

//            renderDistancesProperty.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.FloatField("1", renderDistancesProperty.GetArrayElementAtIndex(0).floatValue);
//            renderDistancesProperty.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.FloatField("2", renderDistancesProperty.GetArrayElementAtIndex(1).floatValue);
//            renderDistancesProperty.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.FloatField("3", renderDistancesProperty.GetArrayElementAtIndex(2).floatValue);
//            renderDistancesProperty.GetArrayElementAtIndex(3).floatValue = EditorGUILayout.FloatField("4", renderDistancesProperty.GetArrayElementAtIndex(3).floatValue);

//            EditorGUILayout.LabelField("");

//            //
//            serializedObject.ApplyModifiedProperties();
//            EditorUtility.SetDirty(self);
//        }
//    }
//}