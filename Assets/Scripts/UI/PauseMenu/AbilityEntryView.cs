using Game.GameControl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
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

        private IGameController GameController;
        private SkillsMenuController SkillsMenuController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(IGameController game_controller, SkillsMenuController skills_menu_controller)
        {
            GameController = game_controller;
            SkillsMenuController = skills_menu_controller;

            NameTextComponent.text = GameController.PlayerModel.AbilityData.GetAbility(AbilityType).Name;
            SetSprite(GameController.PlayerModel.GetAbilityState(AbilityType));

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
}