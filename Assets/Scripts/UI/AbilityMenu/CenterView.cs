using Game.Player.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class CenterView : MonoBehaviour
    {
        //##################################################################

        [SerializeField]
        Image backgroundImage;

        [SerializeField]
        Text favourText;

        [SerializeField]
        Text abilityNameText;

        [SerializeField]
        Text abilityDescriptionText;

        Player.PlayerModel playerModel;
        AbilityMenuController menuController;
        Ability currentAbility;

        //##################################################################

        public void Initialize(Player.PlayerModel playerModel, AbilityMenuController menuController)
        {
            this.playerModel = playerModel;
            this.menuController = menuController;

            favourText.text = playerModel.Favours.ToString();

            abilityNameText.text = string.Empty;
            abilityDescriptionText.text = string.Empty;
            backgroundImage.color = menuController.AvailableAbilityColour;

            Utilities.EventManager.FavourAmountChangedEvent += OnFavourAmountChangedEventHandler;
            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChangedEventHandler;
        }

        public void SetContent(Player.AbilitySystem.Ability ability)
        {
            currentAbility = ability;

            if (ability == null)
            {
                abilityNameText.text = string.Empty;
                abilityDescriptionText.text = string.Empty;
                backgroundImage.color = menuController.AvailableAbilityColour;
            }
            else
            {
                abilityNameText.text = ability.Name;
                abilityDescriptionText.text = ability.Description;

                SetBackgroundColour(playerModel.GetAbilityState(ability.Type));
            }
        }

        //##################################################################

        void OnFavourAmountChangedEventHandler(object sender, Utilities.EventManager.FavourAmountChangedEventArgs args)
        {
            favourText.text = args.FavourAmount.ToString();
        }

        void OnAbilityStateChangedEventHandler(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (currentAbility == null || args.AbilityType != currentAbility.Type)
            {
                return;
            }

            SetBackgroundColour(args.AbilityState);
        }

        //##################################################################

        void SetBackgroundColour(Player.eAbilityState abilityState)
        {
            switch (abilityState)
            {
                case Player.eAbilityState.active:
                    backgroundImage.color = menuController.ActiveAbilityColour;
                    break;
                case Player.eAbilityState.locked:
                    backgroundImage.color = menuController.LockedAbilityColour;
                    break;
                case Player.eAbilityState.available:
                    backgroundImage.color = menuController.AvailableAbilityColour;
                    break;
                default:
                    Debug.LogError("ERROR!");
                    break;
            }
        }

        //##################################################################
    }
} //end of namespace