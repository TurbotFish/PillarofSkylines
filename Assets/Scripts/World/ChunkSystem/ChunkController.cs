using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class ChunkController : MonoBehaviour
    {
        protected ChunkSystemData data;

        protected BoxCollider bounds;
        protected List<SubChunkController> subChunkList = new List<SubChunkController>();

        /// <summary>
        /// Initializes the Chunk.
        /// </summary>
        public virtual void InitializeChunk(WorldController worldController)
        {
            this.data = worldController.ChunkSystemData;

            int childCount = this.transform.childCount;

            //find the bounds
            this.bounds = GetComponent<BoxCollider>();

            if (this.bounds == null)
            {
                Debug.LogErrorFormat("Chunk \"{0}\": could not find bounds collider!", this.name);
            }

            gameObject.layer = 14;
            this.bounds.isTrigger = true;

            //find all the SubChunks
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var subChunk = child.GetComponent<SubChunkController>();

                if (subChunk != null)
                {
                    this.subChunkList.Add(subChunk);
                    subChunk.InitializeSubChunk(worldController);
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
                    if (!subChunk.IsActive)
                    {
                        subChunk.SetSubChunkActive(true);
                    }
                }
                else
                {
                    if (subChunk.IsActive)
                    {
                        subChunk.SetSubChunkActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a copy of the Chunk and attaches it to the given Transform.
        /// </summary>
        public virtual void CreateCopy(Transform parent)
        {
            var chunkCopyGameObject = new GameObject(this.gameObject.name, this.GetType(), typeof(BoxCollider));

            var ChunkCopyTransform = chunkCopyGameObject.transform;
            ChunkCopyTransform.parent = parent;
            ChunkCopyTransform.localPosition = this.transform.localPosition;

            var newBounds = chunkCopyGameObject.GetComponent<BoxCollider>();
            chunkCopyGameObject.layer = gameObject.layer;
            newBounds.isTrigger = bounds.isTrigger;
            newBounds.size = bounds.size;

            foreach (var subChunk in this.subChunkList)
            {
                subChunk.CreateCopy(ChunkCopyTransform);
            }

            chunkCopyGameObject.GetComponent<ChunkController>().InitializeChunkCopy(this);
        }
    }
}