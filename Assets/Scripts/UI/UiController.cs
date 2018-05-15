using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UiController : MonoBehaviour
    {
        //hud controller
        [SerializeField]
        HudController hudController;
        public HudController Hud { get { return this.hudController; } }

        //ability menu controller
        [SerializeField]
        AbilityMenu.AbilityMenuController abilityMenuController;
        public AbilityMenu.AbilityMenuController AbilityMenu { get { return this.abilityMenuController; } }

        //intro menu controller
        [SerializeField]
        IntroMenuController introMenuController;
        public IntroMenuController IntroMenuController { get { return this.introMenuController; } }

        //end menu controller
        [SerializeField]
        EndMenuController endMenuController;
        public EndMenuController EndMenuController { get { return this.endMenuController; } }

        //loading screen controller
        [SerializeField]
        LoadingScreenController loadingScreenController;

        //pillar entrance menu controller
        [SerializeField]
        PillarEntranceMenu.PillarEntranceMenuController pillarEntranceMenuController;

        //help menu controller
        [SerializeField]
        HelpMenuController helpMenu;

        //main menu controller
        [SerializeField]
        MainMenuController mainMenu;



        GameControl.IGameController gameController;

        eUiState currentState = eUiState.NONE;
        Dictionary<eUiState, IUiState> uiStates = new Dictionary<eUiState, IUiState>();

        bool isInitialized = false;

        //###########################################################

        public void InitializeUi(GameControl.IGameController gameController, eUiState startState, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            this.gameController = gameController;

            //
            uiStates.Clear();

            uiStates.Add(eUiState.HUD, hudController);
            uiStates.Add(eUiState.AbilityMenu, abilityMenuController);
            uiStates.Add(eUiState.Intro, introMenuController);
            uiStates.Add(eUiState.End, endMenuController);
            uiStates.Add(eUiState.LoadingScreen, loadingScreenController);
            uiStates.Add(eUiState.PillarEntrance, pillarEntranceMenuController);
            uiStates.Add(eUiState.HelpMenu, helpMenu);
            uiStates.Add(eUiState.MainMenu, mainMenu);

            foreach (var uiState in uiStates.Values)
            {
                uiState.Initialize(this.gameController);
                uiState.Deactivate();
            }

            SwitchState(startState, args);

            Utilities.EventManager.OnShowMenuEvent += OnShowMenuEventHandler;

            isInitialized = true;
        }

        //###########################################################
        //###########################################################

        void SwitchState(eUiState newState, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            //if (this.currentState == newState)
            //{
            //    return;
            //}

            eUiState previousState = currentState;

            if (previousState != eUiState.NONE)
            {
                uiStates[currentState].Deactivate();
            }

            currentState = newState;
            uiStates[currentState].Activate(args);

            Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(newState, previousState));
        }

        void OnShowMenuEventHandler(object sender, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (!isInitialized)
            {
                return;
            }

            SwitchState(args.Menu, args);
        }

        //###########################################################
    }
} //end of namespace