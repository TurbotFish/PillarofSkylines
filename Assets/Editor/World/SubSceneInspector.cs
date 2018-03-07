﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(SubScene))]
    public class SubSceneInspector : Editor
    {
        private SubScene self;

        private SerializedProperty subSceneVariantProperty;
        private SerializedProperty subSceneLayerProperty;

        private void OnEnable()
        {
            self = target as SubScene;

            subSceneVariantProperty = serializedObject.FindProperty("subSceneVariant");
            subSceneLayerProperty = serializedObject.FindProperty("subSceneLayer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Variant", subSceneVariantProperty.enumDisplayNames[subSceneVariantProperty.enumValueIndex]);
            EditorGUILayout.LabelField("Layer", subSceneLayerProperty.enumDisplayNames[subSceneLayerProperty.enumValueIndex]);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Child Count", (self.GetComponentsInChildren<Transform>(true).Length - 1).ToString());
        }
    }
} //end of namespace