using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCamera : MonoBehaviour
    {
        //###############################################################

        // -- ATTRIBUTES

        //[SerializeField] private Vector3 OffsetDirection;
        [SerializeField] private List<DuplicationTrigger> Triggers;

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
                //Debug.LogErrorFormat("{0} enabled!", this.name);
            }
            else if (CameraComponent.enabled && (!CameraManager.IsActive || !is_any_trigger_visible))
            {
                CameraComponent.enabled = false;
                //Debug.LogErrorFormat("{0} disabled!", this.name);
            }

            CameraComponent.fieldOfView = CameraManager.MainCamera.fieldOfView;
            TransformComponent.rotation = CameraManager.MainCameraTransform.rotation;
        }

        private bool CheckIsAnyTriggerVisible()
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

            return is_trigger_visible;
        }
    }
} // end of namespace