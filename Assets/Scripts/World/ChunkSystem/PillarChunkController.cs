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
        public override void InitializeChunk(ChunkSystemData data)
        {
            base.InitializeChunk(data);
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
        public void Activate()
        {
            this.isActive = true;

            this.bounds.gameObject.SetActive(true);
            foreach (var subChunk in this.subChunkList)
            {
                subChunk.ActivateSubChunk();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deactivate()
        {
            this.isActive = false;

            this.bounds.gameObject.SetActive(false);
            foreach (var subChunk in this.subChunkList)
            {
                subChunk.DeactivateSubChunk();
            }
        }
    }
}