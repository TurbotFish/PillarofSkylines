//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [CustomEditor(typeof(RenderDistanceTag))]
//    public class RenderDistanceTagEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            //
//            base.OnInspectorGUI();

//            //
//            var renderDistanceTag = target as RenderDistanceTag;

//            //
//            EditorGUI.BeginChangeCheck();

//            renderDistanceTag.LayerTag = (eSubChunkLayer)EditorGUILayout.EnumPopup("Tag", renderDistanceTag.LayerTag);

//            if (EditorGUI.EndChangeCheck())
//            {
//                MoveObjectIntoCorrectSubChunkRecursively(renderDistanceTag.LayerTag, renderDistanceTag.transform, renderDistanceTag.transform.parent);
//            }

//            //
//            EditorUtility.SetDirty(renderDistanceTag);
//        }

//        void MoveObjectIntoCorrectSubChunkRecursively(eSubChunkLayer targetLayer, Transform objectToMove, Transform recursiveParent)
//        {
//            if (recursiveParent.GetComponent<SubChunkController>() != null)
//            {
//                var chunk = recursiveParent.parent;

//                //look for existing SubChunk
//                int chunkChildCount = chunk.childCount;
//                for (int i = 0; i < chunkChildCount; i++)
//                {
//                    var child = chunk.GetChild(i);
//                    var subChunk = child.GetComponent<SubChunkController>();

//                    if (subChunk != null && subChunk.Layer == targetLayer)
//                    {
//                        objectToMove.SetParent(child);
//                        DeleteEmptySubChunks(chunk);
//                        return;
//                    }
//                }

//                //creating new SubChunk
//                var newSubChunkGo = new GameObject(string.Format("SubChunk_{0}", targetLayer.ToString()), typeof(SubChunkController));
//                newSubChunkGo.transform.SetParent(chunk);

//                var newSubChunk = newSubChunkGo.GetComponent<SubChunkController>();
//                newSubChunk.Editor_InitializeSubChunk(targetLayer);

//                objectToMove.SetParent(newSubChunkGo.transform);
//                DeleteEmptySubChunks(chunk);
//                return;
//            }
//            else if (recursiveParent.GetComponent<ChunkController>() != null)
//            {
//                return;
//            }
//            else if (recursiveParent.parent != null)
//            {
//                MoveObjectIntoCorrectSubChunkRecursively(targetLayer, objectToMove, recursiveParent.parent);
//            }
//        }

//        void DeleteEmptySubChunks(Transform chunk)
//        {
//            int chunkChildCount = chunk.childCount;
//            for (int i = 0; i < chunkChildCount; i++)
//            {
//                var child = chunk.GetChild(i);

//                if (child.GetComponent<SubChunkController>() && child.childCount == 0)
//                {
//                    DestroyImmediate(child.gameObject);
//                }
//            }
//        }
//    }
//}