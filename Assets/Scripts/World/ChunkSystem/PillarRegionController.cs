using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class PillarRegionController : RegionController
    {
        [SerializeField]
        ePillarId pillarId;
        public ePillarId PillarId { get { return this.pillarId; } }

        List<PillarChunkController> intactPillarChunks = new List<PillarChunkController>();
        List<PillarChunkController> destroyedPillarChunks = new List<PillarChunkController>();

        ePillarState currentPillarState = ePillarState.Intact;

        public override void InitializeRegion(ChunkSystemData data)
        {
            base.InitializeRegion(data);

            foreach (var chunk in this.chunkList)
            {
                if (chunk is PillarChunkController)
                {
                    var pillarChunk = chunk as PillarChunkController;

                    if (pillarChunk.PillarState == ePillarState.Intact)
                    {
                        this.intactPillarChunks.Add(pillarChunk);
                    }
                    else if (pillarChunk.PillarState == ePillarState.Destroyed)
                    {
                        this.destroyedPillarChunks.Add(pillarChunk);
                    }
                }
            }

            foreach(var chunk in this.intactPillarChunks)
            {
                this.chunkList.Remove(chunk);
            }

            foreach(var chunk in this.destroyedPillarChunks)
            {
                this.chunkList.Remove(chunk);
            }
        }

        protected override void InitializeRegionCopy(RegionController originalRegion)
        {
            base.InitializeRegionCopy(originalRegion);

            var pillarOriginal = originalRegion as PillarRegionController;

            this.pillarId = pillarOriginal.PillarId;
        }
    }
}