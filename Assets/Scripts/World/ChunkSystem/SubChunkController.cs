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

        public void ActivateSubChunk()
        {
            this.gameObject.SetActive(true);
        }

        public void DeactivateSubChunk()
        {
            this.gameObject.SetActive(false);
        }

        public GameObject CreateCopy(Transform parent)
        {
            if (this.doNotWrap)
            {
                var go = new GameObject(this.gameObject.name);
                go.transform.parent = parent;
                return go;
            }
            else
            {
                var go = Instantiate(this.gameObject, parent);
                return go;
            }
        }
    }
}