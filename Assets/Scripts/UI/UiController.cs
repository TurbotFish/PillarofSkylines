using Game.GameControl;
using Game.UI.AbilityMenu;
using Game.UI.PauseMenu;
using Game.UI.PillarEntranceMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UiController : MonoBehaviour
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private HudController HudController;
        [SerializeField] private LoadingScreenController LoadingScreenController;
        [SerializeField] private PillarEntranceMenuController PillarEntranceMenuController;
        [SerializeField] private MainMenuController MainMenuController;
        [SerializeField] private PauseMenuController PauseMenuController;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsInitialized { get; private set; }
        public MenuType CurrentState { get; private set; }

        private IGameController GameController;

        private Dictionary<MenuType, IUiMenu> UiStates = new Dictionary<MenuType, IUiMenu>();



        //###########################################################

        // -- INITIALIZATION

        public void Initialize(IGameController game_controller, MenuType start_state, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            GameController = game_controller;

            //
            UiStates.Clear();

            UiStates.Add(MenuType.HUD, HudController);
            UiStates.Add(MenuType.LoadingScreen, LoadingScreenController);
            UiStates.Add(MenuType.PillarEntrance, PillarEntranceMenuController);
            UiStates.Add(MenuType.MainMenu, MainMenuController);
            UiStates.Add(MenuType.PauseMenu, PauseMenuController);

            foreach (var uiState in UiStates.Values)
            {
                uiState.Initialize(GameController, this);
                uiState.Deactivate();
            }

            CurrentState = start_state;
            UiStates[CurrentState].Activate(args);
            Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(start_state, MenuType.HUD));

            Utilities.EventManager.OnShowMenuEvent += OnShowMenuEventHandler;

            IsInitialized = true;
        }

        //###########################################################

        // -- INQUIRIES

        public HudController Hud { get { return HudController; } }
        public LoadingScreenController LoadingScreen { get { return LoadingScreenController; } }
        public PillarEntranceMenuController PillarEntranceMenu { get { return PillarEntranceMenuController; } }
        public MainMenuController MainMenu { get { return MainMenuController; } }
        public PauseMenuController PauseMenu { get { return PauseMenuController; } }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            UiStates[CurrentState].HandleInput();
        }

        public void SwitchState(MenuType newState, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            //if (this.currentState == newState)
            //{
            //    return;
            //}

            MenuType previous_state = CurrentState;

            UiStates[CurrentState].Deactivate();

            CurrentState = newState;
            UiStates[CurrentState].Activate(args);

            Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(newState, previous_state));
        }

        public void ExitGame()
        {
            GameController.ExitGame();
        }

        private void OnShowMenuEventHandler(object sender, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (!IsInitialized)
            {
                return;
            }

            SwitchState(args.Menu, args);
        }
    }
} //end of namespace