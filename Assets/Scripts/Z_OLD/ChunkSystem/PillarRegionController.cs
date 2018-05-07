//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    public class PillarRegionController : RegionController
//    {
//        //##################################################################

//        [SerializeField]
//        ePillarId pillarId;
//        public ePillarId PillarId { get { return pillarId; } }

//        List<PillarChunkController> intactPillarChunkList = new List<PillarChunkController>();
//        List<PillarChunkController> destroyedPillarChunkList = new List<PillarChunkController>();
//        List<ChunkController> otherChunkList = new List<ChunkController>();

//        ePillarState currentPillarState = ePillarState.Intact;

//        //##################################################################

//        /// <summary>
//        /// 
//        /// </summary>
//        public override void InitializeRegion(WorldController worldController)
//        {
//            base.InitializeRegion(worldController);


//            for (int i = 0; i < chunkList.Count; i++)
//            {
//                var chunk = chunkList[i];

//                if (chunk is PillarChunkController)
//                {
//                    var pillarChunk = chunk as PillarChunkController;

//                    if (pillarChunk.PillarState == ePillarState.Intact)
//                    {
//                        pillarChunk.ActivatePillarChunk();

//                        intactPillarChunkList.Add(pillarChunk);
//                    }
//                    else if (pillarChunk.PillarState == ePillarState.Destroyed)
//                    {
//                        pillarChunk.DeactivatePillarChunk();

//                        destroyedPillarChunkList.Add(pillarChunk);
//                    }
//                    else
//                    {
//                        Debug.LogError("ERROR!");
//                    }
//                }
//                else
//                {
//                    otherChunkList.Add(chunk);
//                }
//            }

//            Utilities.EventManager.PillarDestroyedEvent += OnPillarDestroyedEventHandler;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected override void InitializeRegionCopy(RegionController originalRegion)
//        {
//            base.InitializeRegionCopy(originalRegion);

//            var pillarOriginal = originalRegion as PillarRegionController;
//            pillarId = pillarOriginal.PillarId;
//        }

//        //##################################################################

//        /// <summary>
//        /// 
//        /// </summary>
//        public override List<Renderer> UpdateChunkSystem(Vector3 playerPos, Vector3 cameraPos, Vector3 cameraForward)
//        {
//            var result = new List<Renderer>();

//            if (currentPillarState == ePillarState.Intact)
//            {
//                for (int i = 0; i < intactPillarChunkList.Count; i++)
//                {
//                    result.AddRange(intactPillarChunkList[i].UpdateChunkSystem(playerPos, cameraPos, cameraForward));
//                }
//            }
//            else if (currentPillarState == ePillarState.Destroyed)
//            {
//                for (int i = 0; i < destroyedPillarChunkList.Count; i++)
//                {
//                    result.AddRange(destroyedPillarChunkList[i].UpdateChunkSystem(playerPos, cameraPos, cameraForward));
//                }
//            }

//            for (int i = 0; i < otherChunkList.Count; i++)
//            {
//                result.AddRange(otherChunkList[i].UpdateChunkSystem(playerPos, cameraPos, cameraForward));
//            }

//            return result;
//        }

//        //##################################################################

//        /// <summary>
//        /// 
//        /// </summary>
//        void OnPillarDestroyedEventHandler(object sender, Utilities.EventManager.PillarDestroyedEventArgs args)
//        {
//            if (args.PillarId != PillarId)
//            {
//                return;
//            }

//            currentPillarState = ePillarState.Destroyed;

//            for (int i = 0; i < intactPillarChunkList.Count; i++)
//            {
//                var chunk = intactPillarChunkList[i];

//                chunk.DeactivatePillarChunk();
//            }

//            for (int i = 0; i < destroyedPillarChunkList.Count; i++)
//            {
//                var chunk = destroyedPillarChunkList[i];

//                chunk.ActivatePillarChunk();
//            }
//        }

//        //##################################################################
//    }
//}