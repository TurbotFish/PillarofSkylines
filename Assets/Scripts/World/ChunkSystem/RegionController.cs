using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class RegionController : MonoBehaviour
    {
        //##################################################################

        protected List<ChunkController> chunkList = new List<ChunkController>();

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeRegion(WorldController worldController)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var chunk = child.GetComponent<ChunkController>();

                if (chunk != null)
                {
                    if (!chunk.gameObject.activeSelf)
                    {
                        chunk.gameObject.SetActive(true);
                    }

                    chunkList.Add(chunk);
                    chunk.InitializeChunk(worldController);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeRegionCopy(RegionController originalRegion)
        {

        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateRegion(Vector3 playerPos, Vector3 cameraPos)
        {
            for (int i = 0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                chunk.UpdateChunk(playerPos, cameraPos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GameObject CreateCopy(Transform parent)
        {
            var go = new GameObject(gameObject.name, GetType());

            var regionCopyTransform = go.transform;
            regionCopyTransform.parent = parent;
            regionCopyTransform.localPosition = transform.localPosition;

            for (int i = 0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                chunk.CreateCopy(regionCopyTransform);
            }

            return go;
        }

        //##################################################################
    }
}