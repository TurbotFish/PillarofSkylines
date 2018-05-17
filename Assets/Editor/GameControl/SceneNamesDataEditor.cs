//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace Game.GameControl
//{
//    [CustomEditor(typeof(SceneNamesData))]
//    public class SceneNamesDataEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            //
//            base.OnInspectorGUI();

//            //
//            var sceneNameData = target as SceneNamesData;

//            sceneNameData.OpenWorldScene_Editor = EditorGUILayout.ObjectField("OpenWorldScene", sceneNameData.OpenWorldScene_Editor, typeof(SceneAsset), false);

//            //
//            EditorGUILayout.LabelField("Pillar Scenes");

//            var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

//            foreach (var pillarId in pillarIdValues)
//            {
//                int index = (int)pillarId;

//                if (sceneNameData.PillarScenes_Editor.Count <= index)
//                {
//                    while (sceneNameData.PillarScenes_Editor.Count <= index)
//                    {
//                        sceneNameData.PillarScenes_Editor.Add(null);
//                    }
//                }

//                sceneNameData.PillarScenes_Editor[index] = EditorGUILayout.ObjectField(pillarId.ToString(), sceneNameData.PillarScenes_Editor[index], typeof(SceneAsset), false);
//            }

//            //
//            sceneNameData.ResetStrings();

//            //
//            EditorUtility.SetDirty(sceneNameData);
//        }
//    }
//}