using Game.GameControl;
using Game.Player;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{

    public class MainMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        public bool IsActive { get; private set; }

        IGameControllerBase gameController;

        [SerializeField]
        private Button playButton;

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

            //
            playButton.Select();
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);

            //
            EventSystem.current.SetSelectedGameObject(null);
        }

        //##################################################################

        // Update is called once per frame
        void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (Input.GetButtonDown("Interact"))
            {
                gameController.StartGame();
            }
        }

        //##################################################################
    }
} //end of namespace