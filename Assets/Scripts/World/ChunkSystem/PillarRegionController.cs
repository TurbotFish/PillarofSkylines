using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class PillarRegionController : RegionController
    {
        //##################################################################

        [SerializeField]
        ePillarId pillarId;
        public ePillarId PillarId { get { return pillarId; } }

        List<PillarChunkController> intactPillarChunks = new List<PillarChunkController>();
        List<PillarChunkController> destroyedPillarChunks = new List<PillarChunkController>();

        ePillarState currentPillarState = ePillarState.Intact;

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeRegion(WorldController worldController)
        {
            base.InitializeRegion(worldController);

            for (int i = 0; i < chunkList.Count; i++)
            {
                var chunk = chunkList[i];

                if (chunk is PillarChunkController)
                {
                    var pillarChunk = chunk as PillarChunkController;

                    if (pillarChunk.PillarState == ePillarState.Intact)
                    {
                        pillarChunk.ActivatePillarChunk();

                        intactPillarChunks.Add(pillarChunk);
                    }
                    else if (pillarChunk.PillarState == ePillarState.Destroyed)
                    {
                        pillarChunk.DeactivatePillarChunk();

                        destroyedPillarChunks.Add(pillarChunk);
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

            pillarId = pillarOriginal.PillarId;
        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateRegion(Vector3 playerPos)
        {
            if (currentPillarState == ePillarState.Intact)
            {
                for (int i = 0; i < intactPillarChunks.Count; i++)
                {
                    var chunk = intactPillarChunks[i];

                    chunk.UpdateChunk(playerPos);
                }
            }
            else if (currentPillarState == ePillarState.Destroyed)
            {
                for (int i = 0; i < destroyedPillarChunks.Count; i++)
                {
                    var chunk = destroyedPillarChunks[i];

                    chunk.UpdateChunk(playerPos);
                }
            }
        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        void OnPillarDestroyedEventHandler(object sender, Utilities.EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId != PillarId)
            {
                return;
            }

            currentPillarState = ePillarState.Destroyed;

            for (int i = 0; i < intactPillarChunks.Count; i++)
            {
                var chunk = intactPillarChunks[i];

                chunk.DeactivatePillarChunk();
            }

            for (int i = 0; i < destroyedPillarChunks.Count; i++)
            {
                var chunk = destroyedPillarChunks[i];

                chunk.ActivatePillarChunk();
            }
        }

        //##################################################################
    }
}