using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class SlotView : MonoBehaviour
    {
        //##################################################################

        [SerializeField]
        Image backgroundImage;

        [SerializeField]
        Outline backgroundOutline;

        [SerializeField]
        Image abilityImage;



        AbilityMenuController menuController;

        public Player.eAbilityType AbilityType { get; private set; }

        //##################################################################

        public void Initialize(Player.PlayerModel playerModel, AbilityMenuController menuController, Player.eAbilityType abilityType)
        {
            this.menuController = menuController;
            AbilityType = abilityType;

            abilityImage.sprite = playerModel.AbilityData.GetAbility(abilityType).Icon;
            SetBackgroundColour(playerModel.GetAbilityState(abilityType));
            backgroundOutline.enabled = false;

            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChangedEventHandler;
        }

        public void SetSelected(bool selected)
        {
            backgroundOutline.enabled = selected;
        }

        //##################################################################

        void OnAbilityStateChangedEventHandler(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (args.AbilityType != AbilityType)
            {
                return;
            }

            SetBackgroundColour(args.AbilityState);
        }

        void SetBackgroundColour(Player.eAbilityState abilityState)
        {
            switch (abilityState)
            {
                case Player.eAbilityState.active:
                    backgroundImage.color = menuController.ActiveAbilityColour;
                    break;
                case Player.eAbilityState.available:
                    backgroundImage.color = menuController.AvailableAbilityColour;
                    break;
                case Player.eAbilityState.locked:
                    backgroundImage.color = menuController.LockedAbilityColour;
                    break;
                default:
                    Debug.LogError("error!");
                    break;
            }
        }

        //##################################################################
    }
} //end of namespace