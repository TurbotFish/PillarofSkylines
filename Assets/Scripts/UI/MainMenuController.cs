using Game.GameControl;
using Game.Player;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.UI
{

    public class MainMenuController : MonoBehaviour, IUiState
    {
        //##################################################################

        public bool IsActive { get; private set; }

        IGameController gameController;

        [SerializeField] private Button playButton;
        [SerializeField] private VideoPlayer VideoPlayer;

        //##################################################################

        void IUiState.Initialize(IGameController gameController)
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

            playButton.Select();
            VideoPlayer.Play();
        }

        void IUiState.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            VideoPlayer.Stop();
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