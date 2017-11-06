using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class PillarChunkController : ChunkController
    {
        [SerializeField]
        ePillarState pillarState;
        public ePillarState PillarState { get { return this.pillarState; } }

        public override void InitializeChunk(ChunkSystemData data)
        {
            base.InitializeChunk(data);
        }

        protected override void InitializeChunkCopy(ChunkController originalChunk)
        {
            base.InitializeChunkCopy(originalChunk);

            var pillarChunkParent = originalChunk as PillarChunkController;

            this.pillarState = pillarChunkParent.PillarState;
        }

        public override void UpdateChunk(Vector3 playerPos)
        {
            base.UpdateChunk(playerPos);
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }
    }
}