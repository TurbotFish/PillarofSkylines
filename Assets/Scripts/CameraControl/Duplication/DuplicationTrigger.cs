using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationTrigger : MonoBehaviour
    {
        //###############################################################

        // -- CONSTANTS

        [SerializeField] private DuplicationAxis _DuplicationAxis;

        //###############################################################

        // -- ATTRIBUTES

        public bool IsVisible { get; private set; }

        private DuplicationCameraManager DuplicationCameraManager;
        private bool IsInitialized;

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(DuplicationCameraManager duplication_camera_manager)
        {
            DuplicationCameraManager = duplication_camera_manager;

            DuplicationCameraManager.OnTriggerStateChanged(this, IsVisible);

            IsInitialized = true;
        }

        //###############################################################

        // -- INQUIRIES

        public DuplicationAxis DuplicationAxis { get { return _DuplicationAxis; } }

        //###############################################################

        // -- OPERATIONS

        private void OnBecameVisible()
        {
            IsVisible = true;

            if (IsInitialized)
            {
                DuplicationCameraManager.OnTriggerStateChanged(this, IsVisible);
            }
        }

        private void OnBecameInvisible()
        {
            IsVisible = false;

            if (IsInitialized)
            {
                DuplicationCameraManager.OnTriggerStateChanged(this, IsVisible);
            }
        }
    }
} // end of namespace