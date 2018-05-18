using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class PillarRegion : RegionBase
    {
        //========================================================================================

        [SerializeField, HideInInspector] private PillarId pillarId;

        //========================================================================================

        public override void Initialize(WorldController world_controller)
        {
            base.Initialize(world_controller);

            EventManager.PillarDestroyedEvent += OnPillarDestroyedEventHandler;
        }

        public override List<SubSceneVariant> AvailableSubSceneVariants
        {
            get
            {
                return new List<SubSceneVariant>() {
                    SubSceneVariant.IntactPillar,
                    SubSceneVariant.DestroyedPillar
                };
            }
        }

        protected override SubSceneVariant InitialSubSceneVariant
        {
            get
            {
                return WorldController.GameController.PlayerModel.CheckIsPillarDestroyed(pillarId) ? SubSceneVariant.DestroyedPillar : SubSceneVariant.IntactPillar;
            }
        }

        //========================================================================================

        void OnPillarDestroyedEventHandler(object sender, EventManager.PillarDestroyedEventArgs args)
        {
            if (args.PillarId != pillarId)
            {
                return;
            }

            SwitchVariant(SubSceneVariant.DestroyedPillar);
        }

    }
} //end of namespace