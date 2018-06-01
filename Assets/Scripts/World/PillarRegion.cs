using Game.Model;
using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class PillarRegion : RegionBase
    {
        //########################################################################

        // -- CONSTANTS

        [SerializeField, HideInInspector] private PillarId pillarId;

        //########################################################################

        // -- INITIALIZATION

        public override void Initialize(WorldController world_controller)
        {
            base.Initialize(world_controller);
        }

        private void OnEnable()
        {
            EventManager.PillarStateChangedEvent += OnPillarStateChanged;
        }

        private void OnDisable()
        {
            EventManager.PillarStateChangedEvent -= OnPillarStateChanged;
        }

        //########################################################################

        // -- INQUIRIES

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
                if (WorldController.GameController.PlayerModel.GetPillarState(pillarId) == PillarState.Destroyed)
                {
                    return SubSceneVariant.DestroyedPillar;
                }

                return SubSceneVariant.IntactPillar;
            }
        }

        //########################################################################

        // -- OPERATIONS

        private void OnPillarStateChanged(object sender, EventManager.PillarStateChangedEventArgs args)
        {
            if (args.PillarId == pillarId)
            {
                if (args.PillarState == PillarState.Destroyed && CurrentSubSceneVariant != SubSceneVariant.DestroyedPillar)
                {
                    SwitchVariant(SubSceneVariant.DestroyedPillar);
                }
                else if (args.PillarState != PillarState.Destroyed && CurrentSubSceneVariant == SubSceneVariant.DestroyedPillar)
                {
                    SwitchVariant(SubSceneVariant.IntactPillar);
                }
            }
        }

    }
} //end of namespace