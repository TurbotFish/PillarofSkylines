using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AbilityMenu
{
    public class CenterView : MonoBehaviour
    {
        //##################################################################

        [SerializeField] private Image backgroundImage;

        //[SerializeField] private Text favourText;
        [SerializeField] private Text pillarKeyText;

        [SerializeField] private TMPro.TextMeshProUGUI abilityNameText;
        [SerializeField] private TMPro.TextMeshProUGUI nameForDescription;
        [SerializeField] private TMPro.TextMeshProUGUI abilityDescriptionText;

        [SerializeField] private DescriptionPanel descriptionPanel;

        private PlayerModel playerModel;
        private AbilityMenuController menuController;

        private Ability currentAbility;
        private bool isActive;

        //##################################################################

        public void Initialize(PlayerModel playerModel, AbilityMenuController menuController)
        {
            this.playerModel = playerModel;
            this.menuController = menuController;

            pillarKeyText.text = playerModel.PillarKeysCount.ToString();

            nameForDescription.text = abilityNameText.text = string.Empty;
            abilityDescriptionText.text = string.Empty;
            backgroundImage.color = menuController.AvailableAbilityColour;
        }

        //##################################################################

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            SetContent(null);

            isActive = false;
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
                //descriptionPanel.gameObject.SetActive(false);
            }
            else
            {
                //descriptionPanel.gameObject.SetActive(true);
                nameForDescription.text = abilityNameText.text = ability.Name;
                abilityDescriptionText.text = ability.Description;

                SetBackgroundColour(playerModel.GetAbilityState(ability.Type));
            }
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            pillarKeyText.text = playerModel.PillarKeysCount.ToString();

            if (currentAbility != null)
            {
                var abilityState = playerModel.GetAbilityState(currentAbility.Type);
                SetBackgroundColour(abilityState);
            }
        }

        //private void OnCurrencyAmountChangedEventHandler(object sender, Utilities.EventManager.CurrencyAmountChangedEventArgs args)
        //{
        //    if (args.CurrencyType == Model.eCurrencyType.Favour)
        //        favourText.text = args.CurrencyAmount.ToString();
        //    else
        //        pillarKeyText.text = args.CurrencyAmount.ToString();
        //}

        //private void OnAbilityStateChangedEventHandler(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        //{
        //    if (currentAbility == null || args.AbilityType != currentAbility.Type)
        //        return;

        //    SetBackgroundColour(args.AbilityState);
        //}

        private void SetBackgroundColour(eAbilityState abilityState)
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