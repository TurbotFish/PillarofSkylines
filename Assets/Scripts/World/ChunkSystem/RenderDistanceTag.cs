using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class RenderDistanceTag : MonoBehaviour
    {
        [SerializeField]
        eSubChunkLayer layerTag;
        public eSubChunkLayer LayerTag { get { return this.layerTag; } }
    }
}
