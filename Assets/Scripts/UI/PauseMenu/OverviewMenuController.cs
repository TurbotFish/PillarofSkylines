using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Model;
using Game.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class OverviewMenuController : MonoBehaviour, IPauseMenu
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

        private PlayerModel Model;
        private PauseMenuController PauseMenuController;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, PauseMenuController pause_menu_controller)
        {
            Model = model;
            PauseMenuController = pause_menu_controller;

            foreach (var pillar_mark_view in PillarMarkHolder.GetComponentsInChildren<PillarMarkIconView>())
            {
                pillar_mark_view.Initialize(Model);
            }

            foreach (var eye_view in EyeHolder.GetComponentsInChildren<EyeIconView>())
            {
                eye_view.Initialize(Model);
            }

            foreach (var ability_view in AbilityHolder.GetComponentsInChildren<AbilityIconView>())
            {
                ability_view.Initialize(Model);
            }
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
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
        }

        public void OnResumeButtonPressed()
        {
            PauseMenuController.ClosePauseMenu();
        }

        public void OnSkillsButtonPressed()
        {
            PauseMenuController.SwitchPauseMenu(PauseMenuType.Skills);
        }

        public void OnOptionsButtonsPressed()
        {
            Debug.LogWarning("Options Button pressed!");
        }

        public void OnExitGameButtonPressed()
        {
            PauseMenuController.ExitGame();
        }
    }
} // end of namespace