using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class ChunkController : MonoBehaviour
    {
        ChunkSystemData data;

        Collider bounds;
        List<SubChunkController> subChunkList = new List<SubChunkController>();

        /// <summary>
        /// Initializes the Chunk.
        /// </summary>
        public virtual void InitializeChunk(ChunkSystemData data)
        {
            this.data = data;

            int childCount = this.transform.childCount;

            //find the bounds
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var bounds = child.GetComponent<Collider>();

                if (bounds != null && bounds.gameObject.layer == 14)
                {
                    this.bounds = bounds;
                    break;
                }
            }
            if (this.bounds == null)
            {
                Debug.LogErrorFormat("Chunk \"{0}\": could not find bounds collider!", this.name);
            }

            //find all the SubChunks
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
        protected virtual void InitializeChunkCopy(ChunkController originalChunk)
        {

        }

        /// <summary>
        /// Update all the things!
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
        /// Creates a copy of the Chunk and attaches it to the given Transform.
        /// </summary>
        public virtual void CreateCopy(Transform parent)
        {
            var chunkCopyGameObject = new GameObject(this.gameObject.name, this.GetType());

            var ChunkCopyTransform = chunkCopyGameObject.transform;
            ChunkCopyTransform.parent = parent;
            ChunkCopyTransform.localPosition = this.transform.localPosition;

            var boundsGo = Instantiate(this.bounds.gameObject, ChunkCopyTransform, true);
            boundsGo.layer = this.bounds.gameObject.layer;           

            foreach (var subChunk in this.subChunkList)
            {
                subChunk.CreateCopy(ChunkCopyTransform);
            }

            chunkCopyGameObject.GetComponent<ChunkController>().InitializeChunkCopy(this);
        }
    }
}