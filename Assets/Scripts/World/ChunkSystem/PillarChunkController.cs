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

        bool isActive = false;

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeChunk(WorldController worldController)
        {
            base.InitializeChunk(worldController);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeChunkCopy(ChunkController originalChunk)
        {
            base.InitializeChunkCopy(originalChunk);

            var pillarChunkParent = originalChunk as PillarChunkController;

            this.pillarState = pillarChunkParent.PillarState;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateChunk(Vector3 playerPos)
        {
            if (this.isActive)
            {
                base.UpdateChunk(playerPos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ActivatePillarChunk()
        {
            this.isActive = true;

            foreach (var subChunk in this.subChunkList)
            {
                subChunk.ActivateSubChunk(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeactivatePillarChunk()
        {
            this.isActive = false;

            foreach (var subChunk in this.subChunkList)
            {
                subChunk.DeactivateSubChunk(true);
            }
        }
    }
}