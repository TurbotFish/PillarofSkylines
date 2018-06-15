using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCamera : MonoBehaviour
    {
        //###############################################################

        // -- ATTRIBUTES

        [SerializeField] private List<DuplicationAxis> TriggerAxes;

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
            CameraComponent.enabled = false;
        }

        //###############################################################

        // -- OPERATIONS

        public void UpdateDuplicationCamera()
        {
            bool is_any_trigger_visible = CheckIsAnyTriggerVisible();

            if (!CameraComponent.enabled && CameraManager.IsActive && is_any_trigger_visible)
            {
                CameraComponent.enabled = true;
            }
            else if (CameraComponent.enabled && (!CameraManager.IsActive || !is_any_trigger_visible))
            {
                CameraComponent.enabled = false;
            }

            CameraComponent.fieldOfView = CameraManager.MainCamera.fieldOfView;
            TransformComponent.rotation = CameraManager.MainCameraTransform.rotation;
        }

        /// <summary>
        /// Checks if any of the triggers for this camera are visible.
        /// </summary>
        /// <returns></returns>
        private bool CheckIsAnyTriggerVisible()
        {
            bool is_trigger_visible = false;

            foreach(var axis in TriggerAxes)
            {
                if (CameraManager.IsDuplicationAxisVisible(axis))
                {
                    is_trigger_visible = true;
                    break;
                }
            }

            return is_trigger_visible;
        }
    }
} // end of namespace