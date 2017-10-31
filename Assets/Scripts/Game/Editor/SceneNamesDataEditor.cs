using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomEditor(typeof(SceneNamesData))]
    public class SceneNamesDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //
            base.OnInspectorGUI();

            //
            var sceneNameData = target as SceneNamesData;

            sceneNameData.OpenWorldScene = EditorGUILayout.ObjectField("OpenWorldScene", sceneNameData.OpenWorldScene, typeof(SceneAsset), false);

            //
            EditorGUILayout.LabelField("Pillar Scenes");

            var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

            foreach(var pillarId in pillarIdValues)
            {
                int index = (int)pillarId;

                if(sceneNameData.PillarScenes.Count <= index)
                {
                    while (sceneNameData.PillarScenes.Count <= index)
                    {
                        sceneNameData.PillarScenes.Add(null);
                    }
                }

                sceneNameData.PillarScenes[index] = EditorGUILayout.ObjectField(pillarId.ToString(), sceneNameData.PillarScenes[index], typeof(SceneAsset), false);
            }

            //
            EditorUtility.SetDirty(sceneNameData);
        }
    }
}