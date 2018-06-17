using Game.Model;
using Game.CameraControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.UI.PauseMenu
{
    public class OptionsPauseMenuController : MonoBehaviour, IPauseMenu
    {
        //###########################################################

        // -- ATTRIBUTES

        [SerializeField] private Button StartingButton;

        [SerializeField] Checkbox invertXBox, invertYBox;


        public bool IsActive { get; private set; }

        private PlayerModel Model;
        private PauseMenuController PauseMenuController;

        PoS_Camera cam;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel model, PauseMenuController pause_menu_controller)
        {
            Model = model;
            PauseMenuController = pause_menu_controller;
        }

        public void Activate()
        {
            this.gameObject.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            StartingButton.Select();

            if (!cam)
                cam = PauseMenuController.GameController.CameraController.PoS_Camera;

            IsActive = true;
        }

        public void Deactivate()
        {
            this.gameObject.SetActive(false);
            IsActive = false;
        }

        //###########################################################

        // -- OPERATIONS

        public void HandleInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                PauseMenuController.SwitchPauseMenu(PauseMenuType.Overview);
            }
        }

        public void OnInvertXAxisPressed()
        {
            cam.invertAxis.x ^= true;

            invertXBox.SetState(cam.invertAxis.x);
        }
        public void OnInvertYAxisPressed()
        {
            cam.invertAxis.y ^= true;

            invertYBox.SetState(cam.invertAxis.y);
        }

    }
}