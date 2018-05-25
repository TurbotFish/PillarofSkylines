using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Utilities;
using UnityEngine;

namespace Game.UI
{
    public class OptionsPauseMenuController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private IGameController GameController;

        //###########################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(IGameController game_controller)
        {
            GameController = game_controller;
        }

        void IUiMenu.Activate(EventManager.OnShowMenuEventArgs args)
        {
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

        public void HandleInput()
        {

        }
    }
}