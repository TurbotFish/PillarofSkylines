using System.Collections.Generic;

namespace Game.World
{
    public class Region : RegionBase
    {
        public override List<eSubSceneMode> AvailableSubSceneModes
        {
            get
            {
                return new List<eSubSceneMode>() {
                    eSubSceneMode.Normal
                };
            }
        }

        protected override eSubSceneMode InitialSubSceneMode
        {
            get
            {
                return eSubSceneMode.Normal;
            }
        }
    }
} //end of namespace