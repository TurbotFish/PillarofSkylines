using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class SubChunkController : MonoBehaviour
    {
        //layer
        [SerializeField]
        eSubChunkLayer layer = 0;
        public eSubChunkLayer Layer { get { return this.layer; } }

        //wrapping
        [SerializeField]
        bool doNotWrap = false;

#if UNITY_EDITOR
        public void Editor_InitializeSubChunk(eSubChunkLayer layer)
        {
            this.layer = layer;
        }
#endif

        //#################

        public void ActivateSubChunk()
        {
            this.gameObject.SetActive(true);
        }

        public void DeactivateSubChunk()
        {
            this.gameObject.SetActive(false);
        }

        void InitializeSubChunkCopy(SubChunkController originalSubChunk)
        {
            this.layer = originalSubChunk.Layer;
        }

        public void CreateCopy(Transform parent)
        {
            if (this.doNotWrap)
            {
                var go = new GameObject(this.gameObject.name);
                go.transform.parent = parent;
                go.transform.localPosition = this.transform.localPosition;

            }
            else
            {
                var go = Instantiate(this.gameObject, parent);

                go.GetComponent<SubChunkController>().InitializeSubChunkCopy(this);
            }
        }
    }
}