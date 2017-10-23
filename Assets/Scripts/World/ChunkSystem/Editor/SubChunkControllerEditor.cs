using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [CustomEditor(typeof(SubChunkController))]
    public class SubChunkControllerEditor : Editor
    {
        eSubChunkLayer flags = 0;

        public override void OnInspectorGUI()
        {
            var subChunkController = target as SubChunkController;

            //
            flags = subChunkController.LayerMask;
            flags = (eSubChunkLayer)EditorGUILayout.EnumMaskField("Layers", flags);

            //
            base.OnInspectorGUI();

            //
            
            subChunkController.LayerMask = flags;

            //
            EditorUtility.SetDirty(subChunkController);
        }
    }
}