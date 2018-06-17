﻿using UnityEngine;
using UnityEngine.UI;
using Game.GameControl;
using System.Collections;
using TMPro;

namespace Game.UI
{

    public class PhotoModeUIController : MonoBehaviour, IUiMenu
    {

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }


        private CanvasGroup MyCanvasGroup;
        private GameController GameController;

        bool IsVisible;

        //###########################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the Hud Menu.
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="ui_controller"></param>
        public void Initialize(GameController gameController, UiController ui_controller) {
            MyCanvasGroup = GetComponent<CanvasGroup>();

            GameController = gameController;
        }

        /// <summary>
        /// Shows the Hud Menu.
        /// </summary>
        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            if (!this.gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            MyCanvasGroup.alpha = 1;
            IsVisible = true;
            IsActive = true;
        }

        /// <summary>
        /// Hides the Hud Menu.
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            if (this.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            
            MyCanvasGroup.alpha = 0;
            IsActive = false;
        }
        
        /// <summary>
        /// Handles Input.
        /// </summary>
        public void HandleInput() {
            if (Input.GetButtonDown("GroundRise"))
            {
                IsVisible ^= true;
                MyCanvasGroup.alpha = IsVisible ? 1 : 0;
            }
        }



    }
}