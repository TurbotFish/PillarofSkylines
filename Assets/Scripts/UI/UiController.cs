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
        [SerializeField] private MainMenuController MainMenuController;
        [SerializeField] private PauseMenuController PauseMenuController;
        [SerializeField] public PhotoModeUIController PhotoModeController;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsInitialized { get; private set; }
        public MenuType CurrentState { get; private set; }

        private GameController GameController;
        private Dictionary<MenuType, IUiMenu> UiStates = new Dictionary<MenuType, IUiMenu>();

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(GameController game_controller)
        {
            GameController = game_controller;

            //
            UiStates.Clear();

            UiStates.Add(MenuType.HUD, HudController);
            UiStates.Add(MenuType.LoadingScreen, LoadingScreenController);
            UiStates.Add(MenuType.MainMenu, MainMenuController);
            UiStates.Add(MenuType.PauseMenu, PauseMenuController);
            UiStates.Add(MenuType.PhotoMode, PhotoModeController);

            foreach (var uiState in UiStates.Values)
            {
                uiState.Initialize(GameController, this);
                uiState.Deactivate();
            }

            CurrentState = MenuType.LoadingScreen;
            UiStates[CurrentState].Activate();

            //Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(start_state, MenuType.HUD));
            //Utilities.EventManager.OnShowMenuEvent += OnShowMenuEventHandler;

            IsInitialized = true;
        }

        //###########################################################

        // -- INQUIRIES

        public HudController Hud { get { return HudController; } }
        public LoadingScreenController LoadingScreen { get { return LoadingScreenController; } }
        public MainMenuController MainMenu { get { return MainMenuController; } }
        public PauseMenuController PauseMenu { get { return PauseMenuController; } }
        public PhotoModeUIController PhotoMode { get { return PhotoModeController; } }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            if (UiStates.ContainsKey(CurrentState))
            {
                UiStates[CurrentState].HandleInput();
            }
        }

        public void SwitchState(MenuType new_state)
        {
            if (CurrentState == new_state)
            {
                return;
            }

            MenuType previous_state = CurrentState;

            if (previous_state != MenuType.NONE)
            {
                UiStates[CurrentState].Deactivate();
            }

            CurrentState = new_state;

            if (new_state != MenuType.NONE)
            {
                UiStates[CurrentState].Activate();
            }
        }

        public void ExitGame()
        {
            GameController.ExitGame();
        }

        //private void OnShowMenuEventHandler(object sender, Utilities.EventManager.OnShowMenuEventArgs args)
        //{
        //    if (!IsInitialized)
        //    {
        //        return;
        //    }

        //    SwitchState(args.Menu, args);
        //}
    }
} //end of namespace