//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [RequireComponent(typeof(BoxCollider))]
//    public class ChunkController : MonoBehaviour
//    {
//        //##################################################################

//        protected ChunkSystemData data;

//        protected BoxCollider chunkBounds;
//        protected List<SubChunkController> subChunkList = new List<SubChunkController>();

//        //##################################################################

//        /// <summary>
//        /// Initializes the Chunk.
//        /// </summary>
//        public virtual void InitializeChunk(WorldController worldController)
//        {
//            data = worldController.ChunkSystemData;

//            gameObject.SetActive(true);

//            //find the bounds
//            chunkBounds = GetComponent<BoxCollider>();

//            if (chunkBounds == null)
//            {
//                Debug.LogErrorFormat("Chunk \"{0}\": could not find bounds collider!", name);
//            }

//            gameObject.layer = 14;
//            chunkBounds.isTrigger = true;

//            //find all the SubChunks
//            int childCount = transform.childCount;
//            for (int i = 0; i < childCount; i++)
//            {
//                var child = transform.GetChild(i);
//                var subChunk = child.GetComponent<SubChunkController>();

//                if (subChunk != null)
//                {
//                    subChunkList.Add(subChunk);
//                    subChunk.InitializeSubChunk(worldController);
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected virtual void InitializeChunkCopy(ChunkController originalChunk)
//        {

//        }

//        //##################################################################

//        /// <summary>
//        /// Update all the things!
//        /// </summary>
//        public virtual List<Renderer> UpdateChunkSystem(Vector3 playerPos, Vector3 cameraPos, Vector3 cameraForward)
//        {
//            var result = new List<Renderer>();
//            float distance;

//            if (chunkBounds.bounds.Contains(playerPos))
//            {
//                distance = 0;
//            }
//            else
//            {
//                var closestPoint = chunkBounds.ClosestPoint(playerPos);
//                distance = Vector3.Distance(playerPos, closestPoint);

//                //check if chunk is behind player
//                var corners = Game.Utilities.PillarMath.GetBoxColliderCorners(chunkBounds);
//                corners.Add(closestPoint); //in case the corners are outside the screen but the collider is in front of the player

//                for (int i = 0; i < corners.Count; i++)
//                {
//                    var cornerDir = (corners[i] - cameraPos).normalized;
//                    float dotProduct = Vector3.Dot(cameraForward, cornerDir);

//                    if (dotProduct < 0)
//                    {
//                        distance *= 15f;
//                        break;
//                    }
//                }
//            }

//            //
//            for (int i = 0; i < subChunkList.Count; i++)
//            {
//                var subChunk = subChunkList[i];
//                var renderDistance = data.GetRenderDistanceInterval(subChunk.Layer);
//                bool newState = false;

//                if (distance >= renderDistance.x && distance < renderDistance.y)
//                {
//                    newState = true;
//                }

//                result.AddRange(subChunk.SetSubChunkActive(newState));
//            }

//            return result;
//        }

//        /// <summary>
//        /// Creates a copy of the Chunk and attaches it to the given Transform.
//        /// </summary>
//        public void CreateCopy(Transform parent)
//        {
//            var chunkCopyGameObject = new GameObject(gameObject.name, GetType(), typeof(BoxCollider));

//            var ChunkCopyTransform = chunkCopyGameObject.transform;
//            ChunkCopyTransform.parent = parent;
//            ChunkCopyTransform.localPosition = transform.localPosition;
//            ChunkCopyTransform.localRotation = transform.localRotation;
//            ChunkCopyTransform.localScale = transform.localScale;

//            var newBounds = chunkCopyGameObject.GetComponent<BoxCollider>();
//            chunkCopyGameObject.layer = gameObject.layer;
//            newBounds.isTrigger = chunkBounds.isTrigger;
//            newBounds.size = chunkBounds.size;
//            newBounds.center = chunkBounds.center;

//            for (int i = 0; i < subChunkList.Count; i++)
//            {
//                var subChunk = subChunkList[i];

//                subChunk.CreateCopy(ChunkCopyTransform);
//            }

//            chunkCopyGameObject.GetComponent<ChunkController>().InitializeChunkCopy(this);
//        }

//        //##################################################################
//    }
//}