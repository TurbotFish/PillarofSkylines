using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCameraManager : MonoBehaviour
    {
        //###############################################################

        // -- ATTRIBUTES

        public Camera MainCamera { get; private set; }
        public Transform MainCameraTransform { get; private set; }
        public bool IsActive { get; private set; }

        [SerializeField] private Transform TriggerHolder;
        [SerializeField] private Transform CameraHolder;

        private List<DuplicationCamera> DuplicationCameras = new List<DuplicationCamera>();

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            MainCamera = gameController.CameraController.PoS_Camera.CameraComponent;
            MainCameraTransform = MainCamera.transform;

            CameraHolder.position = MainCameraTransform.position;

            DuplicationCameras = GetComponentsInChildren<DuplicationCamera>().ToList();
            foreach (var camera in DuplicationCameras)
            {
                camera.Initialize(this);
            }
        }

        //###############################################################

        // -- OPERATIONS

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdateDuplicationCameras()
        {
            if (!IsActive)
            {
                return;
            }

            CameraHolder.position = MainCameraTransform.position;

            foreach (var camera in DuplicationCameras)
            {
                camera.UpdateDuplicationCamera();
            }
        }
    }
} // end of namespace