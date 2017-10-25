using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CustomEditor(typeof(ChunkController))]
    public class ChunkControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var chunkController = target as ChunkController;

            if(GUILayout.Button("Sort into SubChunks"))
            {
                chunkController.SortIntoSubChunks();
            }

            //
            base.OnInspectorGUI();
        }
    }
}