using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingScreenController : MonoBehaviour, IUiState
    {
        [SerializeField] private float turningSpeed;

        private LoadingScreenImages screenImages;
        private Image background;
        private Transform turningThing;

        public bool IsActive { get; private set; }

        //###########################################################

        #region monobehaviour methods

        // Update is called once per frame
        void Update()
        {
            if (IsActive)
            {
                turningThing.Rotate(Vector3.forward * -turningSpeed * Time.unscaledDeltaTime);
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        void IUiState.Initialize(IGameController gameController)
        {
            screenImages = Resources.Load<LoadingScreenImages>("ScriptableObjects/LoadingScreenImages");
            background = transform.Find("Background").GetComponent<Image>();
            turningThing = transform.Find("TurningThing");
        }

        void IUiState.Activate(Utilities.EventManager.OnShowMenuEventArgs args)
        {
            if (IsActive)
            {
                return;
            }

            var realArgs = args as Utilities.EventManager.OnShowLoadingScreenEventArgs;
            if (realArgs != null)
            {
                var sprites = screenImages.GetImages(realArgs.Id);

                if (sprites != null && sprites.Count > 0)
                {
                    background.sprite = sprites[Random.Range(0, sprites.Count - 1)];
                }
            }

            IsActive = true;
            gameObject.SetActive(true);
        }

        void IUiState.Deactivate()
        {
            if (IsActive)
            {
                background.sprite = null;
            }

            IsActive = false;
            gameObject.SetActive(false);
        }

        //###########################################################
    }
}