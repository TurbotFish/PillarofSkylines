using System.Collections;
using System.Collections.Generic;
using Game.GameControl;
using Game.Player;
using Game.Utilities;
using UnityEngine;

namespace Game.UI
{
    public class HelpMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        public bool IsActive { get; private set; }
        private IGameControllerBase gameController;

        //##################################################################

        void IUiState.Initialize(IGameControllerBase gameController)
        {
            this.gameController = gameController;
        }

        //##################################################################

        void IUiState.Activate(EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            gameObject.SetActive(true);
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        //##################################################################     

        // Update is called once per frame
        void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (Input.GetButtonDown("Back"))
            {
                EventManager.SendShowMenuEvent(this, new EventManager.OnShowMenuEventArgs(eUiState.HUD));
                return;
            }
            else if (Input.GetButtonDown("MenuButton"))
            {
                gameController.ExitGame();
            }
        }
    }
}