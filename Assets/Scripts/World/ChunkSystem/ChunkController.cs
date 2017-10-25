using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField]
        Collider bounds;

        ChunkSystemData data;

        List<SubChunkController> subChunkList = new List<SubChunkController>();

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeChunk(ChunkSystemData data)
        {
            this.data = data;

            int childCount = this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var subChunkController = child.GetComponent<SubChunkController>();

                if (subChunkController != null)
                {
                    this.subChunkList.Add(subChunkController);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeChunkCopy(Collider bounds)
        {
            this.bounds = bounds;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateChunk(Vector3 playerPos)
        {
            float distance = 0;

            if (!this.bounds.bounds.Contains(playerPos))
            {
                var closestPoint = this.bounds.ClosestPoint(playerPos);
                distance = Vector3.Distance(playerPos, closestPoint);
            }

            foreach (var subChunk in this.subChunkList)
            {
                var renderDistance = this.data.GetRenderDistance(subChunk.Layer);

                if (distance >= renderDistance.x && distance < renderDistance.y)
                {
                    subChunk.ActivateSubChunk();
                }
                else
                {
                    subChunk.DeactivateSubChunk();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual GameObject CreateCopy(Transform parent)
        {
            var go = new GameObject(this.gameObject.name, this.GetType());
            go.transform.parent = parent;

            var boundsGo = Instantiate(this.bounds.gameObject, go.transform);
            boundsGo.layer = this.bounds.gameObject.layer;

            go.GetComponent<ChunkController>().InitializeChunkCopy(boundsGo.GetComponent<Collider>());

            foreach (var subChunk in this.subChunkList)
            {
                subChunk.CreateCopy(go.transform);
            }

            return go;
        }

#if UNITY_EDITOR
        public void SortIntoSubChunks()
        {
            if(this.bounds == null)
            {
                Debug.LogError("Error: Bounds is null! Set this variable before using this!");
            }

            var subChunkDict = new Dictionary<eSubChunkLayer, Transform>();

            //finding existing subchunks
            int childCount = this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var subChunkController = child.GetComponent<SubChunkController>();

                if (subChunkController != null && !subChunkDict.ContainsKey(subChunkController.Layer))
                {
                    subChunkDict.Add(subChunkController.Layer, subChunkController.transform);
                }
            }

            //creating new subchunks
            var subChunkLayerValues = Enum.GetValues(typeof(eSubChunkLayer)).Cast<eSubChunkLayer>();
            foreach (var layer in subChunkLayerValues)
            {
                if (!subChunkDict.ContainsKey(layer))
                {
                    var subChunkGo = new GameObject(string.Format("SubChunk_{0}", layer.ToString()), typeof(SubChunkController));
                    subChunkGo.transform.SetParent(this.transform);

                    var subChunk = subChunkGo.GetComponent<SubChunkController>();
                    subChunk.Editor_InitializeSubChunk(layer);

                    subChunkDict.Add(layer, subChunkGo.transform);
                }
            }

            //find objects to move
            var objectsToMove = new List<RenderDistanceTag>();

            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);

                if (child.gameObject != this.bounds.gameObject && child.GetComponent<SubChunkController>() == null)
                {
                    var tag = child.GetComponent<RenderDistanceTag>();

                    if (tag == null)
                    {
                        objectsToMove.AddRange(FindRenderDistanceTagsRecursively(child));
                    }
                    else
                    {
                        objectsToMove.Add(tag);
                    }
                }
            }

            //move objects
            foreach (var objectToMove in objectsToMove)
            {
                objectToMove.transform.SetParent(subChunkDict[objectToMove.LayerTag]);
            }

            //delete leftover objects
            childCount = this.transform.childCount;
            var objectsToDelete = new List<GameObject>();

            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);

                if (child.gameObject != this.bounds.gameObject && (child.GetComponent<SubChunkController>() == null || child.childCount == 0))
                {
                    objectsToDelete.Add(child.gameObject);
                }
            }

            foreach (var objectToDelete in objectsToDelete)
            {
                DestroyImmediate(objectToDelete);
            }
        }

        List<RenderDistanceTag> FindRenderDistanceTagsRecursively(Transform parent)
        {
            var result = new List<RenderDistanceTag>();
            int childCount = parent.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                var tag = child.GetComponent<RenderDistanceTag>();

                if (tag == null)
                {
                    result.AddRange(FindRenderDistanceTagsRecursively(child));
                }
                else
                {
                    result.Add(tag);
                }
            }

            return result;
        }
#endif
    }
}