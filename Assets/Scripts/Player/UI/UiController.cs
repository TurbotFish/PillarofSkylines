using Game.Player.UI.AbilityMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{
    public enum eUiState
    {
        HUD,
        AbilityMenu,
        Intro
    }

    public class UiController : MonoBehaviour
    {
        //hud controller
        [SerializeField]
        HudController hudController;

        public HudController Hud { get { return this.hudController; } }

        //ability menu controller
        [SerializeField]
        AbilityMenuController abilityMenuController;

        public AbilityMenuController AbilityMenu { get { return this.abilityMenuController; } }

        //intro menu controller
        [SerializeField]
        IntroMenuController introMenuController;

        public IntroMenuController IntroMenuController { get { return this.introMenuController; } }




        PlayerModel playerModel;

        eUiState currentState = eUiState.HUD;
        Dictionary<eUiState, IUiState> uiStates = new Dictionary<eUiState, IUiState>();

        bool menuButtonDown = false;

        //###########################################################

        public void InitializeUi(PlayerModel playerModel, eUiState startingUiState)
        {
            this.playerModel = playerModel;

            //
            this.uiStates.Clear();

            this.uiStates.Add(eUiState.HUD, this.hudController);
            this.uiStates.Add(eUiState.AbilityMenu, this.abilityMenuController);
            this.uiStates.Add(eUiState.Intro, this.introMenuController);

            foreach (var uiState in uiStates.Values)
            {
                uiState.Initialize(playerModel);
                uiState.Deactivate();
            }

            SwitchState(startingUiState);
        }

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButton("MenuButton") && !this.menuButtonDown)
            {
                switch (this.currentState)
                {
                    case eUiState.HUD:
                        SwitchState(eUiState.AbilityMenu);
                        break;
                    case eUiState.AbilityMenu:
                        SwitchState(eUiState.HUD);
                        break;
                    case eUiState.Intro:
                        SwitchState(eUiState.HUD);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                this.menuButtonDown = true;
            }
            if (!Input.GetButton("MenuButton"))
            {
                this.menuButtonDown = false;
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void SwitchState(eUiState newState)
        {
            //if (this.currentState == newState)
            //{
            //    return;
            //}

            eUiState previousState = this.currentState;

            this.uiStates[this.currentState].Deactivate();
            this.currentState = newState;
            this.uiStates[this.currentState].Activate();

            Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(newState, previousState));
        }

        //###########################################################
    }
} //end of namespace