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

            int pillar_count = Enum.GetValues(typeof(PillarId)).Cast<PillarId>().Count();

            AdjustListSize(Self.PillarSceneObjectList, pillar_count, null);
            AdjustListSize(Self.PillarSceneActivationPriceList, pillar_count, 0);
            AdjustListSize(Self.PillarRewardAbilityList, pillar_count, AbilityType.Dash);
        }

        //###############################################################

        // -- OPERATIONS

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            Self.OpenWorldSceneObject = EditorGUILayout.ObjectField("Open World Scene", Self.OpenWorldSceneObject, typeof(SceneAsset), false);

            EditorGUILayout.LabelField("");

            foreach (var pillar_id in Enum.GetValues(typeof(PillarId)).Cast<PillarId>())
            {
                EditorGUILayout.LabelField("-- " + pillar_id.ToString(), EditorStyles.boldLabel);

                int pillar_index = (int)pillar_id;

                Self.PillarSceneObjectList[pillar_index] = EditorGUILayout.ObjectField("Scene", Self.PillarSceneObjectList[pillar_index], typeof(SceneAsset), false);
                Self.PillarSceneActivationPriceList[pillar_index] = EditorGUILayout.IntField("Activation Price", Self.PillarSceneActivationPriceList[pillar_index]);
                Self.PillarRewardAbilityList[pillar_index] = (AbilityType)EditorGUILayout.EnumPopup("Reward Ability", Self.PillarRewardAbilityList[pillar_index]);
            }

            if (EditorGUI.EndChangeCheck())
            {
                SetNames();

                EditorUtility.SetDirty(Self);
            } 
        }

        private void SetNames()
        {
            if (Self.OpenWorldSceneObject == null)
            {
                Self.OpenWorldSceneName = string.Empty;
            }
            else
            {
                Self.OpenWorldSceneName = Self.OpenWorldSceneObject.name;
            }

            Self.PillarSceneNameList.Clear();
            foreach (var pillar_scene in Self.PillarSceneObjectList)
            {
                if (pillar_scene == null)
                {
                    Self.PillarSceneNameList.Add(string.Empty);
                }
                else
                {
                    Self.PillarSceneNameList.Add(pillar_scene.name);
                }
            }
        }

        private void AdjustListSize<T>(List<T> list, int size, T item)
        {
            while (list.Count != size)
            {
                if (list.Count > size)
                {
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    list.Add(item);
                }
            }
        }
    }
} // end of namespace