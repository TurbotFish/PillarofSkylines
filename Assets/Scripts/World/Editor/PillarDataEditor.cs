using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [CustomEditor(typeof(PillarData))]
    public class PillarDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //
            base.OnInspectorGUI();

            //
            var pillarData = target as PillarData;

            //entry prices
            EditorGUILayout.LabelField("Entry prices");

            var pillarIdValues = Enum.GetValues(typeof(World.ePillarId)).Cast<World.ePillarId>();

            foreach (var pillarId in pillarIdValues)
            {
                int index = (int)pillarId;

                if (pillarData.PillarEntryPriceList.Count <= index)
                {
                    while (pillarData.PillarEntryPriceList.Count <= index)
                    {
                        pillarData.PillarEntryPriceList.Add(0);
                    }
                }

                int newValue = EditorGUILayout.IntField(pillarId.ToString(), pillarData.PillarEntryPriceList[index]);

                if(newValue <= 0)
                {
                    newValue = 0;
                }

                pillarData.PillarEntryPriceList[index] = newValue;
            }

            //
            EditorUtility.SetDirty(pillarData);
        }
    }
}