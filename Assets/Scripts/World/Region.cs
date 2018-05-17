using System.Collections.Generic;

namespace Game.World
{
    public class Region : RegionBase
    {
        public override List<SubSceneVariant> AvailableSubSceneVariants
        {
            get
            {
                return new List<SubSceneVariant>() {
                    SubSceneVariant.Normal
                };
            }
        }

        protected override SubSceneVariant InitialSubSceneVariant
        {
            get
            {
                return SubSceneVariant.Normal;
            }
        }
    }
} //end of namespace