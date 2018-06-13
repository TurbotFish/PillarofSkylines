using Game.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class PillarEntryView : MonoBehaviour, IEntryView, ISelectHandler
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private PillarId PillarId;
        [SerializeField] private Sprite LockedSprite;
        [SerializeField] private Sprite UnlockedSprite;
        [SerializeField] private Sprite DestroyedSprite;
        [SerializeField] private Text NameTextComponent;
        [SerializeField] private Image IconImageComponent;

        //###########################################################

        // -- ATTRIBUTES

        private PlayerModel Model;
        private SkillsMenuController SkillsMenuController;

        private AbilityType AbilityType;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, SkillsMenuController skills_menu_controller)
        {
            Model = model;
            SkillsMenuController = skills_menu_controller;
            AbilityType = Model.LevelData.GetPillarRewardAbility(PillarId);

            NameTextComponent.text = Model.AbilityData.GetAbility(AbilityType).Name;
            SetSprite(Model.GetPillarState(PillarId));

            Utilities.EventManager.PillarStateChangedEvent += OnPillarStateChanged;
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

        private void OnPillarStateChanged(object sender, Utilities.EventManager.PillarStateChangedEventArgs args)
        {
            if (args.PillarId == PillarId)
            {
                SetSprite(args.PillarState);
            }
        }

        private void SetSprite(PillarState pillar_state)
        {
            IconImageComponent.sprite = Model.AbilityData.GetAbility(AbilityType).Icon;
            /*
            switch (pillar_state)
            {
                case PillarState.Locked:
                    IconImageComponent.sprite = LockedSprite;
                    break;
                case PillarState.Unlocked:
                    IconImageComponent.sprite = UnlockedSprite;
                    break;
                case PillarState.Destroyed:
                    IconImageComponent.sprite = DestroyedSprite;
                    break;
            }*/
        }
    }
}