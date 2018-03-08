using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class PillarRegion : RegionBase
    {
        //========================================================================================

        [SerializeField]
        [HideInInspector]
        private ePillarId pillarId;

        //========================================================================================

        public override List<eSubSceneVariant> AvailableSubSceneVariants
        {
            get
            {
                return new List<eSubSceneVariant>() {
                    eSubSceneVariant.IntactPillar,
                    eSubSceneVariant.DestroyedPillar
                };
            }
        }

        protected override eSubSceneVariant InitialSubSceneVariant
        {
            get
            {
                return eSubSceneVariant.IntactPillar;
            }
        }

        //========================================================================================
    }
} //end of namespace