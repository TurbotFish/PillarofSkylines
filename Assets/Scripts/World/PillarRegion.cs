using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class PillarRegion : RegionBase
    {
        //========================================================================================

        [SerializeField, HideInInspector] private ePillarId pillarId;

        //========================================================================================

        public override void Initialize(SuperRegion superRegion)
        {
            base.Initialize(superRegion);

            EventManager.PillarDestroyedEvent += OnPillarDestroyedEventHandler;
        }

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
                return SuperRegion.World.GameController.PlayerModel.CheckIsPillarDestroyed(pillarId) ? eSubSceneVariant.DestroyedPillar : eSubSceneVariant.IntactPillar;
            }
        }

        //========================================================================================

        void OnPillarDestroyedEventHandler(object sender, EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId != pillarId)
            {
                return;
            }

            ChangeSubSceneMode(eSubSceneVariant.DestroyedPillar);
        }

    }
} //end of namespace