using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Model
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataInspector : Editor
    {
        //###############################################################

        // -- ATTRIBUTES

        private LevelData Self;

        //###############################################################

        // -- INITIALIZATION

        private void OnEnable()
        {
            Self = target as LevelData;

            int pillar_count = Enum.GetValues(typeof(World.PillarId)).Cast<World.PillarId>().Count();

            while (Self.PillarSceneObjectList.Count != pillar_count)
            {
                if (Self.PillarSceneObjectList.Count > pillar_count)
                {
                    Self.PillarSceneObjectList.RemoveAt(Self.PillarSceneObjectList.Count - 1);
                }
                else
                {
                    Self.PillarSceneObjectList.Add(null);
                }
            }

            while (Self.PillarSceneActivationPriceList.Count != pillar_count)
            {
                if (Self.PillarSceneActivationPriceList.Count > pillar_count)
                {
                    Self.PillarSceneActivationPriceList.RemoveAt(Self.PillarSceneActivationPriceList.Count - 1);
                }
                else
                {
                    Self.PillarSceneActivationPriceList.Add(0);
                }
            }
        }

        //###############################################################

        // -- OPERATIONS

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Self.OpenWorldSceneObject = EditorGUILayout.ObjectField("Open World Scene", Self.OpenWorldSceneObject, typeof(SceneAsset), false);

            EditorGUILayout.LabelField("");

            foreach (var pillar_id in Enum.GetValues(typeof(World.PillarId)).Cast<World.PillarId>())
            {
                EditorGUILayout.LabelField("-- " + pillar_id.ToString(), EditorStyles.boldLabel);

                int pillar_index = (int)pillar_id;

                Self.PillarSceneObjectList[pillar_index] = EditorGUILayout.ObjectField("Scene", Self.PillarSceneObjectList[pillar_index], typeof(SceneAsset), false);
                Self.PillarSceneActivationPriceList[pillar_index] = EditorGUILayout.IntField("Activation Price", Self.PillarSceneActivationPriceList[pillar_index]);
            }

            Self.SetNames();

            EditorUtility.SetDirty(Self);
        }
    }
} // end of namespace