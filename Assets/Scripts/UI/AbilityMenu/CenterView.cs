﻿using Game.Model;
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
        Text pillarKeyText;

        [SerializeField]
        TMPro.TextMeshProUGUI abilityNameText;
        [SerializeField]
        TMPro.TextMeshProUGUI nameForDescription;

        [SerializeField]
        TMPro.TextMeshProUGUI abilityDescriptionText;

        PlayerModel playerModel;
        AbilityMenuController menuController;
        Ability currentAbility;

        DescriptionPanel descriptionPanel;

        //##################################################################

        public void Initialize(PlayerModel playerModel, AbilityMenuController menuController)
        {
            this.playerModel = playerModel;
            this.menuController = menuController;

            favourText.text = playerModel.GetCurrencyAmount(Model.eCurrencyType.Favour).ToString();
            pillarKeyText.text = playerModel.GetCurrencyAmount(Model.eCurrencyType.PillarKey).ToString();

            nameForDescription.text = abilityNameText.text = string.Empty;
            abilityDescriptionText.text = string.Empty;
            backgroundImage.color = menuController.AvailableAbilityColour;

            descriptionPanel = abilityDescriptionText.transform.parent.GetComponent<DescriptionPanel>();

            Utilities.EventManager.CurrencyAmountChangedEvent += OnCurrencyAmountChangedEventHandler;
            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChangedEventHandler;
        }

        public void SetContent(Ability ability)
        {
            if (currentAbility != ability)
                descriptionPanel.Appear();

            currentAbility = ability;

            if (ability == null)
            {
                nameForDescription.text = abilityNameText.text = string.Empty;
                abilityDescriptionText.text = string.Empty;
                backgroundImage.color = menuController.AvailableAbilityColour;
                descriptionPanel.gameObject.SetActive(false);
            }
            else
            {
                descriptionPanel.gameObject.SetActive(true);
                nameForDescription.text = abilityNameText.text = ability.Name;
                abilityDescriptionText.text = ability.Description;

                SetBackgroundColour(playerModel.GetAbilityState(ability.Type));
            }
        }

        //##################################################################

        void OnCurrencyAmountChangedEventHandler(object sender, Utilities.EventManager.CurrencyAmountChangedEventArgs args)
        {
            if (args.CurrencyType == Model.eCurrencyType.Favour)
                favourText.text = args.CurrencyAmount.ToString();
            else
                pillarKeyText.text = args.CurrencyAmount.ToString();
        }

        void OnAbilityStateChangedEventHandler(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (currentAbility == null || args.AbilityType != currentAbility.Type)
                return;

            SetBackgroundColour(args.AbilityState);
        }

        //##################################################################

        void SetBackgroundColour(eAbilityState abilityState)
        {
            switch (abilityState)
            {
                case eAbilityState.active:
                    backgroundImage.color = menuController.ActiveAbilityColour;
                    break;
                case eAbilityState.locked:
                    backgroundImage.color = menuController.LockedAbilityColour;
                    break;
                case eAbilityState.available:
                    backgroundImage.color = menuController.AvailableAbilityColour;
                    break;
                case eAbilityState.pillarLocked:
                    backgroundImage.color = menuController.PillarLockedAbilityColour;
                    break;
                default:
                    Debug.LogError("ERROR!");
                    break;
            }
        }

        //##################################################################
    }
} //end of namespace