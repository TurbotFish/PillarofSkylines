//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.World
//{
//    public class SuperRegion : MonoBehaviour
//    {
//        eSuperRegionType type;
//        WorldController world;

//        List<RegionBase> regions = new List<RegionBase>();

//        //========================================================================================

//        public eSuperRegionType Type { get { return type; } }
//        public WorldController World { get { return world; } }

//        //========================================================================================

//        public void Initialize(eSuperRegionType type, WorldController world, List<RegionBase> regions)
//        {
//            this.type = type;
//            this.world = world;

//            this.regions = new List<RegionBase>(regions);

//            foreach (var region in this.regions)
//            {
//                region.Initialize(this);
//            }
//        }

//        public List<SubSceneJob> UpdateSuperRegion(Transform cameraTransform, Vector3 playerPosition, List<Vector3> teleportPositions)
//        {
//            var result = new List<SubSceneJob>();
//            foreach (var region in regions)
//            {
//                result.AddRange(region.UpdateRegion(cameraTransform, playerPosition, teleportPositions));
//            }

//            result.RemoveAll(item => item == null);
//            return result;
//        }

//        public List<SubSceneJob> UnloadAll()
//        {
//            var result = new List<SubSceneJob>();
//            foreach (var region in regions)
//            {
//                result.AddRange(region.UnloadAll());
//            }

//            result.RemoveAll(item => item == null);
//            return result;
//        }
//    }
//} //end of namespace