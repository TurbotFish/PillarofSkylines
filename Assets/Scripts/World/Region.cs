using System.Collections.Generic;

namespace Game.World
{
    public class Region : RegionBase
    {
        public override List<eSubSceneVariant> AvailableSubSceneVariants
        {
            get
            {
                return new List<eSubSceneVariant>() {
                    eSubSceneVariant.Normal
                };
            }
        }

        protected override eSubSceneVariant InitialSubSceneVariant
        {
            get
            {
                return eSubSceneVariant.Normal;
            }
        }
    }
} //end of namespace