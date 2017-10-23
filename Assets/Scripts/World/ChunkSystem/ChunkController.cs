using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField]
        BoxCollider bounds;

        ChunkSystemData data;

        List<SubChunkController> subChunks = new List<SubChunkController>();

        public void Initialize(ChunkSystemData data)
        {
            this.data = data;

        }

        public void UpdateChunk(Vector3 playerPos)
        {

        }
    }
}