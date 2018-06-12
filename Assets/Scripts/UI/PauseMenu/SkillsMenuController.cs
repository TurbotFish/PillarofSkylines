using Game.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class SkillsMenuController : MonoBehaviour, IPauseMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private Transform EntryHolder;
        [SerializeField] private Button StartingButton;
        [SerializeField] private DescriptionPanelView DescriptionPanelView;
        [SerializeField] private AbilityListView AbilityListView;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private PauseMenuController PauseMenuController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, PauseMenuController pause_menu_controller)
        {
            PauseMenuController = pause_menu_controller;

            foreach (var entry in EntryHolder.GetComponentsInChildren<IEntryView>())
            {
                entry.Initialize(model, this);
            }

            DescriptionPanelView.Initialize(model);
            AbilityListView.Initialize(model);
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);

            StartingButton.Select();

            IsActive = true;
        }

        public void Deactivate()
        {
            this.gameObject.SetActive(false);
            IsActive = false;
        }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                PauseMenuController.SwitchPauseMenu(PauseMenuType.Overview);
            }
        }

        public void OnAbilitySelected(AbilityType ability)
        {
            DescriptionPanelView.ShowAbility(ability);
        }
    }
} // end of namespace