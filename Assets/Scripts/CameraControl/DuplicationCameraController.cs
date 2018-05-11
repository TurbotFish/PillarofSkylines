using Game.GameControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCameraController : MonoBehaviour
    {
        private Camera MainCamera;
        private Transform MainCameraTransform;

        private List<Camera> DuplicationCameraList;
        private List<Transform> DuplicationCameraTransformList;

        private bool isActive;
        private bool areDuplicationCamerasEnabled;

        // -- INITIALIZATION

        public void Initialize(IGameControllerBase gameController)
        {
            MainCamera = gameController.CameraController.PoS_Camera.CameraComponent;
            MainCameraTransform = MainCamera.transform;
            transform.position = MainCameraTransform.position;

            DuplicationCameraList = GetComponentsInChildren<Camera>().ToList();
            DuplicationCameraTransformList = new List<Transform>();
            foreach (var camera in DuplicationCameraList)
            {
                DuplicationCameraTransformList.Add(camera.transform);
            }
        }

        // -- OPERATIONS

        public void Activate()
        {
            isActive = true;

            EnableCameras();
        }

        public void Deactivate()
        {
            isActive = false;

            EnableCameras();
        }

        public void UpdateDuplicationCameras()
        {
            if (!isActive)
            {
                return;
            }

            EnableCameras();

            transform.position = MainCameraTransform.position;

            foreach (var camera in DuplicationCameraList)
            {
                camera.fieldOfView = MainCamera.fieldOfView;
            }

            foreach (var camera_transform in DuplicationCameraTransformList)
            {
                camera_transform.rotation = MainCameraTransform.rotation;
            }
        }

        private void EnableCameras()
        {
            bool switchNeeded = false;

            if (isActive && MainCamera.enabled)
            {
                if (!areDuplicationCamerasEnabled)
                {
                    areDuplicationCamerasEnabled = true;
                    switchNeeded = true;
                }
            }
            else if (areDuplicationCamerasEnabled)
            {
                areDuplicationCamerasEnabled = false;
                switchNeeded = true;
            }

            if (switchNeeded)
            {
                foreach (var camera in DuplicationCameraList)
                {
                    camera.enabled = areDuplicationCamerasEnabled;
                }
            }
        }
    }
} // end of namespace