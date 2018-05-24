using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AbilityIconView : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private AbilityType AbilityType;
        [SerializeField] private Sprite InactiveSprite;
        [SerializeField] private Sprite ActiveSprite;

        //###########################################################

        // -- ATTRIBUTES

        private Image ImageComponent;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(IGameController game_controller)
        {
            ImageComponent = GetComponent<Image>();

            SetSprite(game_controller.PlayerModel.GetAbilityState(AbilityType));

            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChanged;
        }

        private void OnDestroy()
        {
            Utilities.EventManager.AbilityStateChangedEvent -= OnAbilityStateChanged;
        }

        //###########################################################

        // -- OPERATIONS

        private void OnAbilityStateChanged(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            throw new System.NotImplementedException();
        }

        private void SetSprite(AbilityState ability_state)
        {
            switch (ability_state)
            {
                case AbilityState.inactive:
                    ImageComponent.sprite = InactiveSprite;
                    break;
                case AbilityState.active:
                    ImageComponent.sprite = ActiveSprite;
                    break;
            }
        }
    }
} // end of namespace