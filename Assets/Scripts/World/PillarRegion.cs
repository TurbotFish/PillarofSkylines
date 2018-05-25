using Game.Model;
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

            EventManager.PillarStateChangedEvent += OnPillarStateChanged;
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
                if(WorldController.GameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed)
                {
                    return SubSceneVariant.DestroyedPillar;
                }

                return SubSceneVariant.IntactPillar;
            }
        }

        //========================================================================================

        private void OnPillarStateChanged(object sender, EventManager.PillarStateChangedEventArgs args)
        {
           if(args.PillarId == pillarId)
            {
                if(args.PillarState == PillarState.Destroyed && CurrentSubSceneVariant != SubSceneVariant.DestroyedPillar)
                {
                    SwitchVariant(SubSceneVariant.DestroyedPillar);
                }
                else if(args.PillarState != PillarState.Destroyed && CurrentSubSceneVariant == SubSceneVariant.DestroyedPillar)
                {
                    SwitchVariant(SubSceneVariant.IntactPillar);
                }
            }
        }

    }
} //end of namespace