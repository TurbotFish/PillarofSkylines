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

            GUILayout.Label("Sort Chunk content (see tooltip)");

            if(GUILayout.Button(new GUIContent( "Sort into SubChunks", "This will create SubChunks and move GameObject with the component \"RenderDistanceTag\" into them.")))
            {
                chunkController.SortIntoSubChunks();
            }

            //
            base.OnInspectorGUI();
        }
    }
}