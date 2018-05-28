using Game.GameControl;
using Game.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.UI
{

    public class MainMenuController : MonoBehaviour, IUiMenu
    {
        //##################################################################

        // -- CONSTANTS

        [SerializeField] private Button playButton;
        [SerializeField] private VideoPlayer VideoPlayer;

        //##################################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        IGameController gameController;

        //##################################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(IGameController gameController, UiController ui_controller)
        {
            this.gameController = gameController;
        }

        void IUiMenu.Activate(EventManager.OnShowMenuEventArgs args)
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

        void IUiMenu.Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            VideoPlayer.Stop();
        }

        //##################################################################

        // -- OPERATIONS

        public bool HandleInput()
        {
            if (Input.GetButtonDown("Interact"))
            {
                gameController.StartGame();
            }

            return true;
        }
    }
} //end of namespace