using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class PillarChunkController : ChunkController
    {
        //pillar layer
        [SerializeField]
        ePillarState pillarState;
        public ePillarState PillarState { get { return this.pillarState; } }

        public override void InitializeChunk(ChunkSystemData data)
        {
            base.InitializeChunk(data);
        }

        public override void UpdateChunk(Vector3 playerPos)
        {
            base.UpdateChunk(playerPos);
        }
    }
}