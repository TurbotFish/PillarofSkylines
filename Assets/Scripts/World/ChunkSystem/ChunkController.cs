using System.Collections;
using System.Collections.Generic;
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

            foreach(var subChunk in this.subChunkList)
            {
                subChunk.CreateCopy(go.transform);
            }

            return go;
        }
    }
}