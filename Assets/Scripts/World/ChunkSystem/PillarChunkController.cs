using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.ChunkSystem
{
    public class PillarChunkController : ChunkController
    {
        //##################################################################

        [SerializeField]
        ePillarState pillarState;
        public ePillarState PillarState { get { return pillarState; } }

        bool isActive = false;

        //##################################################################

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

            pillarState = pillarChunkParent.PillarState;
        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateChunk(Vector3 playerPos)
        {
            if (isActive)
            {
                base.UpdateChunk(playerPos);
            }
        }

        //##################################################################

        /// <summary>
        /// 
        /// </summary>
        public void ActivatePillarChunk()
        {
            isActive = true;

            for (int i = 0; i < subChunkList.Count; i++)
            {
                var subChunk = subChunkList[i];

                subChunk.SetSubChunkActive(true, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeactivatePillarChunk()
        {
            isActive = false;

            for (int i = 0; i < subChunkList.Count; i++)
            {
                var subChunk = subChunkList[i];

                subChunk.SetSubChunkActive(false, true);
            }
        }

        //##################################################################
    }
}