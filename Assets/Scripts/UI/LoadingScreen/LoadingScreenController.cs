using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingScreenController : MonoBehaviour, IUiMenu
    {
        //###########################################################

        // -- CONSTANTS

        [SerializeField] private float turningSpeed;

        //###########################################################

        // -- ATTRIBUTES

        public bool IsActive { get; private set; }

        private GameController GameController;
        private LoadingScreenImages screenImages;
        private Image background;
        private Transform turningThing;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController, UiController ui_controller)
        {
            GameController = gameController;
            screenImages = Resources.Load<LoadingScreenImages>("ScriptableObjects/LoadingScreenImages");
            background = transform.Find("Background").GetComponent<Image>();
            turningThing = transform.Find("TurningThing");
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            int loading_screen_id = -1;
            if (GameController.IsSwitchingToPillar)
            {
                loading_screen_id = (int)GameController.ActivePillarId;
            }

            var sprites = screenImages.GetImages(loading_screen_id);

            if (sprites != null && sprites.Count > 0)
            {
                background.sprite = sprites[Random.Range(0, sprites.Count - 1)];
            }


            IsActive = true;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                background.sprite = null;
            }

            IsActive = false;
            gameObject.SetActive(false);
        }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
        }

        void Update()
        {
            if (IsActive)
            {
                turningThing.Rotate(Vector3.forward * -turningSpeed * Time.unscaledDeltaTime);
            }
        }
    }
}