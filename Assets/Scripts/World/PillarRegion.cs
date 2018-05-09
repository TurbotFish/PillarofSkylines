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

        public override void Initialize(WorldController world_controller)
        {
            base.Initialize(world_controller);

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
                return WorldController.GameController.PlayerModel.CheckIsPillarDestroyed(pillarId) ? eSubSceneVariant.DestroyedPillar : eSubSceneVariant.IntactPillar;
            }
        }

        //========================================================================================

        void OnPillarDestroyedEventHandler(object sender, EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId != pillarId)
            {
                return;
            }

            SwitchVariant(eSubSceneVariant.DestroyedPillar);
        }

    }
} //end of namespace