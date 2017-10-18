﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI
{

    public class UiController : MonoBehaviour
    {
        enum eUiState
        {
            HUD,
            AbilityMenu,
            Intro
        }

        //###########################################################

        [SerializeField]
        HudController hudController;

        public HudController Hud { get { return this.hudController; } }

        [SerializeField]
        AbilityMenuController abilityMenuController;

        public AbilityMenuController AbilityMenu { get { return this.abilityMenuController; } }

        [SerializeField]
        float timeScaleChangeTime = 0.5f;




        eUiState currentState = eUiState.HUD;
        Dictionary<eUiState, IUiState> uiStates = new Dictionary<eUiState, IUiState>();

        //###########################################################

        #region monobehaviour methods

        // Use this for initialization
        void Start()
        {
            this.uiStates.Add(eUiState.HUD, this.hudController);
            this.uiStates.Add(eUiState.AbilityMenu, this.abilityMenuController);

            foreach (var uiState in uiStates.Values)
            {
                uiState.Deactivate();
            }

            this.uiStates[this.currentState].Activate();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("AbilityMenuButton"))
            {
                switch (this.currentState)
                {
                    case eUiState.HUD:
                        SwitchState(eUiState.AbilityMenu);
                        StartCoroutine(ChangeTimeScaleRoutine(0, this.timeScaleChangeTime));
                        break;
                    case eUiState.AbilityMenu:
                        SwitchState(eUiState.HUD);
                        StartCoroutine(ChangeTimeScaleRoutine(1, this.timeScaleChangeTime));
                        break;
                    default:
                        throw new NotImplementedException();
                }
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
            float initialValue = Time.timeScale;
            float changePerSecond = (targetValue - initialValue) / changeTime;

            while (Time.timeScale != targetValue)
            {
                float newTimeScale = Time.timeScale - (Time.deltaTime * changePerSecond);

                if ((initialValue < targetValue) && (newTimeScale > targetValue) ||
                    (initialValue > targetValue) && (newTimeScale < targetValue))
                {
                    newTimeScale = targetValue;
                }

                Time.timeScale = newTimeScale;

                yield return null;
            }
        }

        //###########################################################
    }
} //end of namespace