﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class ChunkController : MonoBehaviour
    {
        //##################################################################

        protected ChunkSystemData data;

        protected BoxCollider bounds;
        protected List<SubChunkController> subChunkList = new List<SubChunkController>();

        //##################################################################

        /// <summary>
        /// Initializes the Chunk.
        /// </summary>
        public virtual void InitializeChunk(WorldController worldController)
        {
            data = worldController.ChunkSystemData;

            gameObject.SetActive(true);

            //find the bounds
            bounds = GetComponent<BoxCollider>();

            if (bounds == null)
            {
                Debug.LogErrorFormat("Chunk \"{0}\": could not find bounds collider!", name);
            }

            gameObject.layer = 14;
            bounds.isTrigger = true;

            //find all the SubChunks
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var subChunk = child.GetComponent<SubChunkController>();

                if (subChunk != null)
                {
                    subChunkList.Add(subChunk);
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

        //##################################################################

        /// <summary>
        /// Update all the things!
        /// </summary>
        public virtual void UpdateChunk(Vector3 playerPos, Vector3 cameraPos)
        {
            var corners = Utilities.PillarMath.GetBoxColliderCorners(bounds);
            var cameraDir = (playerPos - cameraPos).normalized;
            bool colliderVisible = false;
            float distance = 0;

            for (int i = 0; i < corners.Count; i++)
            {
                var cornerDir = (corners[i] - cameraPos).normalized;
                float dotProduct = Vector3.Dot(cameraDir, cornerDir);

                if(dotProduct > 0)
                {
                    colliderVisible = true;
                    break;
                }
            }

            if (!colliderVisible)
            {
                Debug.LogFormat("Chunk \"{0}\" behind camera!", name);
                distance = float.MaxValue;
            }
            else if (!this.bounds.bounds.Contains(playerPos))
            {
                var closestPoint = bounds.ClosestPoint(playerPos);
                distance = Vector3.Distance(playerPos, closestPoint);
            }

            for (int i = 0; i < subChunkList.Count; i++)
            {
                var subChunk = subChunkList[i];

                var renderDistance = data.GetRenderDistance(subChunk.Layer);

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
        public void CreateCopy(Transform parent)
        {
            var chunkCopyGameObject = new GameObject(gameObject.name, GetType(), typeof(BoxCollider));

            var ChunkCopyTransform = chunkCopyGameObject.transform;
            ChunkCopyTransform.parent = parent;
            ChunkCopyTransform.localPosition = transform.localPosition;

            var newBounds = chunkCopyGameObject.GetComponent<BoxCollider>();
            chunkCopyGameObject.layer = gameObject.layer;
            newBounds.isTrigger = bounds.isTrigger;
            newBounds.size = bounds.size;
            newBounds.center = bounds.center;

            for (int i = 0; i < subChunkList.Count; i++)
            {
                var subChunk = subChunkList[i];

                subChunk.CreateCopy(ChunkCopyTransform);
            }

            chunkCopyGameObject.GetComponent<ChunkController>().InitializeChunkCopy(this);
        }

        //##################################################################
    }
}