using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCamera : MonoBehaviour
    {
        //###############################################################

        // -- ATTRIBUTES

        [SerializeField] private List<DuplicationPlane> Triggers;

        private Transform TransformComponent;
        private Camera CameraComponent;

        private DuplicationCameraManager CameraManager;

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(DuplicationCameraManager camera_controller)
        {
            CameraManager = camera_controller;

            TransformComponent = transform;
            CameraComponent = GetComponent<Camera>();
        }

        //###############################################################

        // -- OPERATIONS

        public void UpdateDuplicationCamera()
        {
            bool is_trigger_visible = false;

            foreach (var trigger in Triggers)
            {
                if (trigger.IsVisible)
                {
                    is_trigger_visible = true;
                    break;
                }
            }

            if (!CameraComponent.enabled && CameraManager.IsActive && is_trigger_visible)
            {
                CameraComponent.enabled = true;
            }
            else if (CameraComponent.enabled && !(CameraManager.IsActive || is_trigger_visible))
            {
                CameraComponent.enabled = false;
            }

            CameraComponent.fieldOfView = CameraManager.MainCamera.fieldOfView;
            TransformComponent.rotation = CameraManager.MainCameraTransform.rotation;
        }
    }
} // end of namespace