using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class SlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Debug.LogFormat("OnPointerEnter: slot \"{0}\"", AbilityType.ToString());

            menuController.OnPointerEnterSlot(this);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Debug.LogFormat("OnPointerExit: slot \"{0}\"", AbilityType.ToString());

            menuController.OnPointerExitSlot(this);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Debug.LogFormat("OnPointerClick: slot \"{0}\"", AbilityType.ToString());

            menuController.OnPointerClickSlot(this);
        }
    }
} //end of namespace