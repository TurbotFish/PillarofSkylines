using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class OverviewPillarMarkView : MonoBehaviour
    {
        [SerializeField] private PillarMarkId PillarMarkId;
        [SerializeField] private Sprite ActiveSprite;
        [SerializeField] private Sprite InactiveSprite;

        private Image ImageComponent;

        public void Initialize(IGameController game_controller)
        {
            ImageComponent = GetComponent<Image>();

            SetSprite(game_controller.PlayerModel.GetPillarMarkState(PillarMarkId));

            Utilities.EventManager.PillarMarkStateChangedEvent += OnPillarMarkStateChanged;
        }

        private void OnPillarMarkStateChanged(object sender, Utilities.EventManager.PillarMarkStateChangedEventArgs args)
        {
            if(PillarMarkId == args.PillarMarkId)
            {
                SetSprite(args.PillarMarkState);
            }
        }

        private void SetSprite(PillarMarkState state)
        {
            switch (state)
            {
                case PillarMarkState.inactive:
                    ImageComponent.sprite = InactiveSprite;
                    break;
                case PillarMarkState.active:
                    ImageComponent.sprite = ActiveSprite;
                    break;
            }
        }
    }
} // end of namespace