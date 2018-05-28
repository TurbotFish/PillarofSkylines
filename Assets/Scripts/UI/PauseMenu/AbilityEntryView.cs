using Game.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class AbilityEntryView : MonoBehaviour, IEntryView, ISelectHandler
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private AbilityType AbilityType;
        [SerializeField] private Sprite InactiveSprite;
        [SerializeField] private Sprite ActiveSprite;
        [SerializeField] private Text NameTextComponent;
        [SerializeField] private Image IconImageComponent;

        //###########################################################

        // -- ATTRIBUTES

        private PlayerModel Model;
        private SkillsMenuController SkillsMenuController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, SkillsMenuController skills_menu_controller)
        {
            Model = model;
            SkillsMenuController = skills_menu_controller;

            NameTextComponent.text = Model.AbilityData.GetAbility(AbilityType).Name;
            SetSprite(Model.GetAbilityState(AbilityType));

            Utilities.EventManager.AbilityStateChangedEvent += OnAbilityStateChanged;
        }

        //###########################################################

        // -- OPERATIONS

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == this.gameObject)
            {
                SkillsMenuController.OnAbilitySelected(AbilityType);
            }
        }

        private void OnAbilityStateChanged(object sender, Utilities.EventManager.AbilityStateChangedEventArgs args)
        {
            if (args.AbilityType == AbilityType)
            {
                SetSprite(args.AbilityState);
            }
        }

        private void SetSprite(AbilityState ability_state)
        {
            switch (ability_state)
            {
                case AbilityState.inactive:
                    IconImageComponent.sprite = InactiveSprite;
                    break;
                case AbilityState.active:
                    IconImageComponent.sprite = ActiveSprite;
                    break;
            }
        }
    }
} // end of namespace