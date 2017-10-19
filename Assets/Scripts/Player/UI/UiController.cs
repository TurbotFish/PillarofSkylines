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

        [SerializeField]
        float timeScaleChangeTime = 0.5f;

        [SerializeField]
        float menuTimescale = 0.1f;




        PlayerModel playerModel;

        eUiState currentState = eUiState.HUD;
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

            foreach (var uiState in uiStates.Values)
            {
                uiState.Initialize(playerModel);
                uiState.Deactivate();
            }

            (this.hudController as IUiState).Activate();
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
                        StartCoroutine(ChangeTimeScaleRoutine(this.menuTimescale, this.timeScaleChangeTime));
                        break;
                    case eUiState.AbilityMenu:
                        SwitchState(eUiState.HUD);
                        StartCoroutine(ChangeTimeScaleRoutine(1, this.timeScaleChangeTime));
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
            if (this.currentState == newState)
            {
                return;
            }

            this.uiStates[this.currentState].Deactivate();
            this.currentState = newState;
            this.uiStates[this.currentState].Activate();
        }

        IEnumerator ChangeTimeScaleRoutine(float targetValue, float changeTime)
        {
            if (targetValue < 0)
            {
                targetValue = 0;
            }

            float initialValue = Time.timeScale;
            float changePerSecond = (targetValue - initialValue) / changeTime;

            //Debug.LogFormat("ChangeTimeScaleRoutine: initialValue={0}, targetValue={1}, changePerSecond={2}", initialValue, targetValue, changePerSecond);

            while (Time.timeScale != targetValue)
            {
                float newTimeScale = Time.timeScale + (Time.deltaTime * changePerSecond);

                if ((initialValue < targetValue) && (newTimeScale > targetValue) ||
                    (initialValue > targetValue) && (newTimeScale < targetValue))
                {
                    newTimeScale = targetValue;
                }
                else if (newTimeScale < 0)
                {
                    newTimeScale = 0;
                }

                //Debug.LogFormat("ChangeTimeScaleRoutine: newTimeScale={0}", newTimeScale);
                Time.timeScale = newTimeScale;

                yield return null;
            }
        }

        IEnumerator ShowIntroRoutine()
        {
            yield return null;

            SwitchState(eUiState.Intro);
        }

        //###########################################################
    }
} //end of namespace