using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Model;
using Game.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SkillsMenuController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private Transform EntryHolder;
        [SerializeField] private Button StartingButton;
        [SerializeField] private DescriptionPanelView DescriptionPanelView;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private IGameController GameController;

        //###########################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(IGameController game_controller)
        {
            GameController = game_controller;

            foreach (var entry in EntryHolder.GetComponentsInChildren<IEntryView>())
            {
                entry.Initialize(game_controller, this);
            }

            DescriptionPanelView.Initialize(game_controller);
        }

        void IUiMenu.Activate(EventManager.OnShowMenuEventArgs args)
        {
            //StartCoroutine(SelectStartingButton());

            this.gameObject.SetActive(true);

            StartingButton.Select();

            IsActive = true;
        }

        void IUiMenu.Deactivate()
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
                GameController.UiController.PauseMenu.SwitchMenu(PauseMenuType.Overview);
            }
        }

        public void OnAbilitySelected(AbilityType ability)
        {
            DescriptionPanelView.ShowAbility(ability);
        }

        private IEnumerator SelectStartingButton()
        {
            yield return null;
            StartingButton.Select();
        }
    }
} // end of namespace