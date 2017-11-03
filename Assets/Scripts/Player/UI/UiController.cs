using Game.Player.UI.AbilityMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{
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




        PlayerModel playerModel;

        eUiState currentState = eUiState.LoadingScreen;
        Dictionary<eUiState, IUiState> uiStates = new Dictionary<eUiState, IUiState>();

        bool menuButtonDown = false;

        //###########################################################

        public void InitializeUi(PlayerModel playerModel)
        {
            this.playerModel = playerModel;

            //
            this.uiStates.Clear();

            this.uiStates.Add(eUiState.HUD, this.hudController);
            this.uiStates.Add(eUiState.AbilityMenu, this.abilityMenuController);
            this.uiStates.Add(eUiState.Intro, this.introMenuController);
            this.uiStates.Add(eUiState.End, this.endMenuController);
            this.uiStates.Add(eUiState.LoadingScreen, this.loadingScreenController);
            this.uiStates.Add(eUiState.PillarEntrance, this.pillarEntranceMenuController);

            foreach (var uiState in uiStates.Values)
            {
                if((object)uiState != this.loadingScreenController)
                uiState.Initialize(playerModel);
                uiState.Deactivate();
            }
        }

        //###########################################################

        #region monobehaviour methods

        void OnEnable()
        {
            if(this.playerModel == null) //this is only executed if the instance has not been initialized yet
            {
                (this.loadingScreenController as IUiState).Activate(null);
                this.currentState = eUiState.LoadingScreen;
            }
        }

        void Start()
        {
            Utilities.EventManager.OnShowMenuEvent += OnShowMenuEventHandler;
        }

        #endregion monobehaviour methods

        //###########################################################

        void SwitchState(eUiState newState, Utilities.EventManager.OnShowMenuEventArgs args = null)
        {
            //if (this.currentState == newState)
            //{
            //    return;
            //}

            eUiState previousState = this.currentState;

            this.uiStates[this.currentState].Deactivate();
            this.currentState = newState;
            this.uiStates[this.currentState].Activate(args);

            Utilities.EventManager.SendOnMenuSwitchedEvent(this, new Utilities.EventManager.OnMenuSwitchedEventArgs(newState, previousState));
        }

        void OnShowMenuEventHandler(object sender, Utilities.EventManager.OnShowMenuEventArgs args)
        {
            SwitchState(args.Menu, args);
        }

        //###########################################################
    }
} //end of namespace