using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class SubChunkController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        eSubChunkLayer layerMask = 0;
        public eSubChunkLayer LayerMask { get { return this.layerMask; } set { this.layerMask = value; } }

        public void ActivateSubChunk()
        {
            this.gameObject.SetActive(true);
        }

        public void DeactivateSubChunk()
        {
            this.gameObject.SetActive(false);
        }
    }
}