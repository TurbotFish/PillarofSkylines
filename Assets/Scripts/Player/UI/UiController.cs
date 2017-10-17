using System;
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
            AbilityMenu
        }

        //###########################################################

        [SerializeField]
        HudController hudController;

        [SerializeField]
        AbilityMenuController abilityMenuController;



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
            if (Input.GetKeyDown(KeyCode.X))
            {
                switch (this.currentState)
                {
                    case eUiState.HUD:
                        SwitchState(eUiState.AbilityMenu);
                        break;
                    case eUiState.AbilityMenu:
                        SwitchState(eUiState.HUD);
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
    }
} //end of namespace