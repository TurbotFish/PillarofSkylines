using System.Collections.Generic;
using UnityEngine;

namespace Game.World.New
{
    public class PillarRegion : RegionBase
    {
        //========================================================================================

        [SerializeField]
        [HideInInspector]
        private ePillarId pillarId;

        //========================================================================================

        public override List<eSubSceneMode> AvailableSubSceneModes
        {
            get
            {
                return new List<eSubSceneMode>() {
                    eSubSceneMode.IntactPillar,
                    eSubSceneMode.DestroyedPillar
                };
            }
        }

        protected override eSubSceneMode InitialSubSceneMode
        {
            get
            {
                return eSubSceneMode.IntactPillar;
            }
        }

        //========================================================================================
    }
} //end of namespace