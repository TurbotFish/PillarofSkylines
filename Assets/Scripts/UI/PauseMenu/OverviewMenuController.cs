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
        [SerializeField] private Transform PillarAbilityHolder;
        [SerializeField] private Transform LuluHolder;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private PlayerModel Model;
        private PauseMenuController PauseMenuController;

        private bool ResumeButtonPressed;
        private bool SkillsButtonPressed;
        private bool OptionsButtonPressed;
        private bool ExitGameButtonPressed;

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

            foreach (var ability_view in PillarAbilityHolder.GetComponentsInChildren<AbilityIconView>())
            {
                ability_view.Initialize(Model);
            }

            foreach (var lulu_view in LuluHolder.GetComponentsInChildren<LuluIconView>())
            {
                lulu_view.Initialize(Model);
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
            if (ResumeButtonPressed)
            {
                ResumeButtonPressed = false;
                PauseMenuController.ClosePauseMenu();
            }
            else if (SkillsButtonPressed)
            {
                SkillsButtonPressed = false;
                PauseMenuController.SwitchPauseMenu(PauseMenuType.Skills);
            }
            else if (OptionsButtonPressed)
            {
                OptionsButtonPressed = false;
                PauseMenuController.SwitchPauseMenu(PauseMenuType.Options);
            }
            else if (ExitGameButtonPressed)
            {
                ExitGameButtonPressed = false;
                PauseMenuController.ExitGame();
            }
        }

        public void OnResumeButtonPressed()
        {
            ResumeButtonPressed = true;
        }

        public void OnSkillsButtonPressed()
        {
            SkillsButtonPressed = true;
        }

        public void OnOptionsButtonsPressed()
        {
            OptionsButtonPressed = true;
        }

        public void OnExitGameButtonPressed()
        {
            ExitGameButtonPressed = true;
        }
    }
} // end of namespace