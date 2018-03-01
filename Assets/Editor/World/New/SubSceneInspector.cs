using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.World.New
{
    [CustomEditor(typeof(SubScene))]
    public class SubSceneInspector : Editor
    {
        private SubScene self;

        private SerializedProperty subSceneModeProperty;
        private SerializedProperty subSceneTypeProperty;

        private void OnEnable()
        {
            self = target as SubScene;

            subSceneModeProperty = serializedObject.FindProperty("subSceneMode");
            subSceneTypeProperty = serializedObject.FindProperty("subSceneType");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Mode", subSceneModeProperty.enumDisplayNames[subSceneModeProperty.enumValueIndex]);
            EditorGUILayout.LabelField("Type", subSceneTypeProperty.enumDisplayNames[subSceneTypeProperty.enumValueIndex]);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Child Count", (self.GetComponentsInChildren<Transform>(true).Length - 1).ToString());
        }
    }
} //end of namespace