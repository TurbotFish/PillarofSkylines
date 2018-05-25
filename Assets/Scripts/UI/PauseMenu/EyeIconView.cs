using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EyeIconView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private PillarId PillarId;
        [SerializeField] private Sprite LockedSprite;
        [SerializeField] private Sprite UnlockedSprite;
        [SerializeField] private Sprite DestroyedSprite;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(IGameController game_controller)
        {
            ImageComponent = GetComponent<Image>();

            SetSprite(game_controller.PlayerModel.GetPillarState(PillarId));

            Utilities.EventManager.PillarStateChangedEvent += OnPillarStateChanged;
        }

        private void OnDestroy()
        {
            Utilities.EventManager.PillarStateChangedEvent -= OnPillarStateChanged;
        }

        //###########################################################

        // -- OPERATIONS

        private void OnPillarStateChanged(object sender, Utilities.EventManager.PillarStateChangedEventArgs args)
        {
            if (PillarId == args.PillarId)
            {
                SetSprite(args.PillarState);
            }
        }

        private void SetSprite(PillarState pillar_state)
        {
            switch (pillar_state)
            {
                case PillarState.Locked:
                    ImageComponent.sprite = LockedSprite;
                    break;
                case PillarState.Unlocked:
                    ImageComponent.sprite = UnlockedSprite;
                    break;
                case PillarState.Destroyed:
                    ImageComponent.sprite = DestroyedSprite;
                    break;
            }
        }
    }
} // end of namespace