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

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeRegion(WorldController worldController)
        {
            base.InitializeRegion(worldController);

            foreach (var chunk in this.chunkList)
            {
                if (chunk is PillarChunkController)
                {
                    var pillarChunk = chunk as PillarChunkController;

                    if (pillarChunk.PillarState == ePillarState.Intact)
                    {
                        pillarChunk.ActivatePillarChunk();

                        this.intactPillarChunks.Add(pillarChunk);
                    }
                    else if (pillarChunk.PillarState == ePillarState.Destroyed)
                    {
                        pillarChunk.DeactivatePillarChunk();

                        this.destroyedPillarChunks.Add(pillarChunk);
                    }
                }
            }

            Utilities.EventManager.PillarDestroyedEvent += OnPillarDestroyedEventHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeRegionCopy(RegionController originalRegion)
        {
            base.InitializeRegionCopy(originalRegion);

            var pillarOriginal = originalRegion as PillarRegionController;

            this.pillarId = pillarOriginal.PillarId;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateRegion(Vector3 playerPos)
        {
            if(this.currentPillarState == ePillarState.Intact)
            {
                foreach(var chunk in this.intactPillarChunks)
                {
                    chunk.UpdateChunk(playerPos);
                }
            }
            else if(this.currentPillarState == ePillarState.Destroyed)
            {
                foreach(var chunk in this.destroyedPillarChunks)
                {
                    chunk.UpdateChunk(playerPos);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnPillarDestroyedEventHandler(object sender, Utilities.EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId != this.PillarId)
            {
                return;
            }

            this.currentPillarState = ePillarState.Destroyed;

            foreach (var chunk in this.intactPillarChunks)
            {
                chunk.DeactivatePillarChunk();
            }

            foreach (var chunk in this.destroyedPillarChunks)
            {
                chunk.ActivatePillarChunk();
            }
        }
    }
}