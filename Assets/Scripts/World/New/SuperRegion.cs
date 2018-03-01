using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.New
{
    public class SuperRegion : MonoBehaviour
    {
        eSuperRegionType type;
        WorldController world;

        List<RegionBase> regions = new List<RegionBase>();

        //========================================================================================

        public eSuperRegionType Type { get { return type; } }
        public WorldController World { get { return world; } }

        //========================================================================================

        public void Initialize(eSuperRegionType type, WorldController world, List<RegionBase> regions)
        {
            this.type = type;
            this.world = world;

            this.regions = new List<RegionBase>(regions);

            foreach (var region in this.regions)
            {
                region.Initialize(this);
            }
        }

        public List<SubSceneJob> UpdateSuperRegion(Transform cameraTransform, List<Vector3> teleportPositions)
        {
            var result = new List<SubSceneJob>();
            foreach (var region in regions)
            {
                result.AddRange(region.UpdateRegion(cameraTransform, teleportPositions));
            }

            result.RemoveAll(item => item == null);
            return result;
        }

        //========================================================================================

        //        #region editor methods

        //#if UNITY_EDITOR

        //        [ExecuteInEditMode]
        //        public bool IsRegionNameUnique(string regionName)
        //        {
        //            Transform myTransform = transform;
        //            int occurences = 0;

        //            for (int i = 0; i < myTransform.childCount; i++)
        //            {
        //                var child = myTransform.GetChild(i);

        //                if (child.GetComponent<RegionBase>() != null && child.name == regionName)
        //                {
        //                    occurences++;
        //                }
        //            }

        //            if (occurences > 1)
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }

        //#endif

        //        #endregion editor methods

        //========================================================================================
    }
}