using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CustomEditor(typeof(ChunkSystemData))]
    public class ChunkSystemDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //
            base.OnInspectorGUI();

            //
            var chunkSystemData = target as ChunkSystemData;

            //
            EditorGUILayout.LabelField("Render Distances");

            var subChunkLayerValues = Enum.GetValues(typeof(eSubChunkLayer)).Cast<eSubChunkLayer>();

            foreach (var layer in subChunkLayerValues)
            {
                chunkSystemData.SetRenderDistance(layer, EditorGUILayout.Vector2Field(layer.ToString(), chunkSystemData.GetRenderDistance(layer)));
            }

            EditorGUILayout.LabelField("");

            //
            EditorUtility.SetDirty(chunkSystemData);
        }
    }
}