using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class RegionController : MonoBehaviour
    {
        protected List<ChunkController> chunkList = new List<ChunkController>();

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeRegion(ChunkSystemData data)
        {
            int childCount = this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = this.transform.GetChild(i);
                var chunk = child.GetComponent<ChunkController>();

                if (chunk != null)
                {
                    this.chunkList.Add(chunk);
                    chunk.InitializeChunk(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeRegionCopy(RegionController originalRegion)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateRegion(Vector3 playerPos)
        {
            foreach (var chunk in this.chunkList)
            {
                chunk.UpdateChunk(playerPos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual GameObject CreateCopy(Transform parent)
        {
            var go = new GameObject(this.gameObject.name, this.GetType());

            var regionCopyTransform = go.transform;
            regionCopyTransform.parent = parent;
            regionCopyTransform.localPosition = this.transform.localPosition;          

            foreach(var chunk in this.chunkList)
            {
                chunk.CreateCopy(regionCopyTransform);
            }

            return go;
        }
    }
}