//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World.ChunkSystem
//{
//    [CreateAssetMenu(menuName = "ScriptableObjects/ChunkSystemData", fileName = "ChunkSystemData")]
//    public class ChunkSystemData : ScriptableObject
//    {
//        const int RENDER_DISTANCE_COUNT = 4;

//        //##################################################################

//        [SerializeField]
//        [HideInInspector]
//        List<float> renderDistances = new List<float>(RENDER_DISTANCE_COUNT);

//        [SerializeField]
//        float minUpdateDistance = 1f;
//        public float MinUpdateDistance { get { return minUpdateDistance; } }

//        [Tooltip("The amount of regions that are updated every frame. This is for the chunk system.")]
//        [SerializeField]
//        int regionUpdatesPerFrame;
//        public int RegionUpdatesPerFrame { get { return regionUpdatesPerFrame; } }

//        [Tooltip("The amount of gameobjects in subchunks that are activated or deactivated every frame.")]
//        [SerializeField]
//        int objectActivationsPerFrame;
//        public int ObjectActivationsPerFrame { get { return objectActivationsPerFrame; } }

//        //##################################################################

//        public Vector2 GetRenderDistanceInterval(eSubChunkLayer layer)
//        {
//            var result = Vector2.zero;
//            var parts = layer.ToString().Split('_');

//            result.x = GetRenderDistance(parts[0]);
//            result.y = GetRenderDistance(parts[1]);

//            return result;
//        }

//        float GetRenderDistance(string name)
//        {
//            switch (name)
//            {
//                case "Inside":
//                    return 0;
//                case "Infinity":
//                    return float.MaxValue;
//                case "One":
//                    return renderDistances[0];
//                case "Two":
//                    return renderDistances[1];
//                case "Three":
//                    return renderDistances[2];
//                case "Four":
//                    return renderDistances[3];
//                default:
//                    Debug.LogErrorFormat("abc:{0}", name);
//                    return float.MaxValue;
//            }
//        }

//        //public void SetRenderDistance(eSubChunkLayer layer, Vector2 value)
//        //{
//        //    if (!Application.isPlaying)
//        //    {
//        //        if (value.x <= 0)
//        //        {
//        //            value.x = 0;
//        //        }
//        //        if (value.y <= 0)
//        //        {
//        //            value.y = 0;
//        //        }

//        //        if (renderDistances.Count > (int)layer)
//        //        {
//        //            renderDistances[(int)layer] = value;
//        //        }
//        //        else
//        //        {
//        //            while(renderDistances.Count <= (int)layer)
//        //            {
//        //                renderDistances.Add(Vector2.zero);
//        //            }

//        //            renderDistances[(int)layer] = value;
//        //        }
//        //    }
//        //}

//        //######################################################

//        void OnValidate()
//        {
//            while(renderDistances.Count != RENDER_DISTANCE_COUNT)
//            {
//                if(renderDistances.Count < RENDER_DISTANCE_COUNT)
//                {
//                    renderDistances.Add(0);
//                }
//                else if(renderDistances.Count > RENDER_DISTANCE_COUNT)
//                {
//                    renderDistances.RemoveAt(renderDistances.Count - 1);
//                }
//            }
//        }

//        //######################################################
//    }
//}