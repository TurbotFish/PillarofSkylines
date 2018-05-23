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

            foreach(var pillar_mark_view in PillarMarkHolder.GetComponentsInChildren<OverviewPillarMarkView>())
            {
                pillar_mark_view.Initialize(game_controller);
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
            Debug.Log("Resume Button pressed!");
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
            Debug.Log("Exit Game Button pressed!");
        }
    }
} // end of namespace