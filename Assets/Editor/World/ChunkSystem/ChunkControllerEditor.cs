//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [CustomEditor(typeof(ChunkController))]
//    public class ChunkControllerEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            GUILayout.Label("Sort Chunk content (see tooltip)");

//            if(GUILayout.Button(new GUIContent( "Sort into SubChunks", "This will create SubChunks and move GameObject with the component \"RenderDistanceTag\" into them.")))
//            {
//                SortIntoSubChunks();
//            }

//            //
//            base.OnInspectorGUI();
//        }

//        void SortIntoSubChunks()
//        {
//            var chunkController = target as ChunkController;

//            var subChunkDict = new Dictionary<eSubChunkLayer, Transform>();

//            //finding existing subchunks
//            int childCount = chunkController.transform.childCount;
//            for (int i = 0; i < childCount; i++)
//            {
//                var child = chunkController.transform.GetChild(i);
//                var subChunkController = child.GetComponent<SubChunkController>();

//                if (subChunkController != null && !subChunkDict.ContainsKey(subChunkController.Layer))
//                {
//                    subChunkDict.Add(subChunkController.Layer, subChunkController.transform);
//                }
//            }

//            //creating new subchunks
//            var subChunkLayerValues = Enum.GetValues(typeof(eSubChunkLayer)).Cast<eSubChunkLayer>();
//            foreach (var layer in subChunkLayerValues)
//            {
//                if (!subChunkDict.ContainsKey(layer))
//                {
//                    var subChunkGo = new GameObject(string.Format("SubChunk_{0}", layer.ToString()), typeof(SubChunkController));
//                    subChunkGo.transform.SetParent(chunkController.transform);

//                    var subChunk = subChunkGo.GetComponent<SubChunkController>();
//                    subChunk.Editor_InitializeSubChunk(layer);

//                    subChunkDict.Add(layer, subChunkGo.transform);
//                }
//            }

//            //find objects to move
//            var objectsToMove = new List<RenderDistanceTag>();

//            for (int i = 0; i < childCount; i++)
//            {
//                var child = chunkController.transform.GetChild(i);

//                if (child.gameObject.layer != 14 && child.GetComponent<SubChunkController>() == null)
//                {
//                    var tag = child.GetComponent<RenderDistanceTag>();

//                    if (tag == null)
//                    {
//                        objectsToMove.AddRange(FindRenderDistanceTagsRecursively(child));
//                    }
//                    else
//                    {
//                        objectsToMove.Add(tag);
//                    }
//                }
//            }

//            //move objects
//            foreach (var objectToMove in objectsToMove)
//            {
//                objectToMove.transform.SetParent(subChunkDict[objectToMove.LayerTag]);
//            }

//            //delete leftover objects
//            childCount = chunkController.transform.childCount;
//            var objectsToDelete = new List<GameObject>();

//            for (int i = 0; i < childCount; i++)
//            {
//                var child = chunkController.transform.GetChild(i);

//                if (child.gameObject.layer != 14 && (child.GetComponent<SubChunkController>() == null || child.childCount == 0))
//                {
//                    objectsToDelete.Add(child.gameObject);
//                }
//            }

//            foreach (var objectToDelete in objectsToDelete)
//            {
//                DestroyImmediate(objectToDelete);
//            }
//        }

//        List<RenderDistanceTag> FindRenderDistanceTagsRecursively(Transform parent)
//        {
//            var result = new List<RenderDistanceTag>();
//            int childCount = parent.childCount;

//            for (int i = 0; i < childCount; i++)
//            {
//                var child = parent.GetChild(i);
//                var tag = child.GetComponent<RenderDistanceTag>();

//                if (tag == null)
//                {
//                    result.AddRange(FindRenderDistanceTagsRecursively(child));
//                }
//                else
//                {
//                    result.Add(tag);
//                }
//            }

//            return result;
//        }
//    }
//}