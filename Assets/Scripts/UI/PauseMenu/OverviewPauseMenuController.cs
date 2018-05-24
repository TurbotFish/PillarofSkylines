using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class OverviewPauseMenuController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private Button StartingButton;
        [SerializeField] private Transform PillarMarkHolder;
        [SerializeField] private Transform EyeHolder;
        [SerializeField] private Transform AbilityHolder;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private IGameController GameController;

        //###########################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(IGameController game_controller)
        {
            GameController = game_controller;

            foreach(var pillar_mark_view in PillarMarkHolder.GetComponentsInChildren<PillarMarkIconView>())
            {
                pillar_mark_view.Initialize(game_controller);
            }

            foreach(var eye_view in EyeHolder.GetComponentsInChildren<EyeIconView>())
            {
                eye_view.Initialize(game_controller);
            }

            foreach(var ability_view in AbilityHolder.GetComponentsInChildren<AbilityIconView>())
            {
                ability_view.Initialize(game_controller);
            }
        }

        void IUiMenu.Activate(EventManager.OnShowMenuEventArgs args)
        {
            StartingButton.Select();

            this.gameObject.SetActive(true);
            IsActive = true;
        }

        void IUiMenu.Deactivate()
        {
            this.gameObject.SetActive(false);
            IsActive = false;
        }

        //###########################################################

        // -- OPERATIONS

        public void OnResumeButtonPressed()
        {
            GameController.UiController.SwitchState(MenuType.HUD, new EventManager.OnShowMenuEventArgs(MenuType.HUD));
        }

        public void OnSkillsButtonPressed()
        {
            Debug.Log("Skills Button pressed!");
        }

        public void OnOptionsButtonsPressed()
        {
            Debug.Log("Options Button pressed!");
        }

        public void OnExitGameButtonPressed()
        {
            GameController.ExitGame();
        }
    }
} // end of namespace