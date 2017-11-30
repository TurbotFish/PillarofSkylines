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
        }

        public void SetContent(Player.AbilitySystem.Ability ability)
        {
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

                switch (playerModel.GetAbilityState(ability.Type))
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
        }

        //##################################################################

        void OnFavourAmountChangedEventHandler(object sender, Utilities.EventManager.FavourAmountChangedEventArgs args)
        {
            favourText.text = args.FavourAmount.ToString();
        }

        //##################################################################
    }
} //end of namespace