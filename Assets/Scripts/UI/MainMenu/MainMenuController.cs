using Game.GameControl;
using Game.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using TMPro;

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

        private GameController GameController;

        //##################################################################

        // -- INITIALIZATION

        void IUiMenu.Initialize(GameController gameController, UiController ui_controller)
        {
            this.GameController = gameController;
        }
        
        void IUiMenu.Activate()
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

        void Start()
        {
            StartCoroutine(_AppearLeBouton());
        }

        IEnumerator _AppearLeBouton()
        {
            TextMeshProUGUI txt = playButton.GetComponent<TextMeshProUGUI>();
            Color color = txt.color;
            
            float duration = 7.0f;

            for(float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {

                color.a = elapsed / duration;
                txt.color = color;
                yield return null;
            }
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

        public void HandleInput()
        {
            if (Input.GetButtonDown("Interact"))
            {
                GameController.StartGame();
            }
        }
    }
} //end of namespace