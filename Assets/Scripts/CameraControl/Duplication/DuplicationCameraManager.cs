using Game.GameControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CameraControl
{
    public class DuplicationCameraManager : MonoBehaviour
    {
        //###############################################################

        // -- CONSTANTS

        [SerializeField] private Transform TriggerHolder;
        [SerializeField] private Transform CameraHolder;

        //###############################################################

        // -- ATTRIBUTES

        public Camera MainCamera { get; private set; }
        public Transform MainCameraTransform { get; private set; }
        public bool IsActive { get; private set; }

        private GameController GameController;

        private List<DuplicationCamera> DuplicationCameras = new List<DuplicationCamera>();
        private Dictionary<DuplicationAxis, List<DuplicationTrigger>> VisibleDuplicationTriggerDictionary = new Dictionary<DuplicationAxis, List<DuplicationTrigger>>();
        private Dictionary<DuplicationAxis, bool> DuplicationTriggerProximityDictionary = new Dictionary<DuplicationAxis, bool>();

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            GameController = gameController;

            MainCamera = GameController.CameraController.PoS_Camera.CameraComponent;
            MainCameraTransform = MainCamera.transform;

            CameraHolder.position = MainCameraTransform.position;

            /*
             * Initialize cameras
             */
            DuplicationCameras = CameraHolder.GetComponentsInChildren<DuplicationCamera>().ToList();
            foreach (var camera in DuplicationCameras)
            {
                camera.Initialize(this);
            }

            /*
             * Initialize dictionaries
             */
            foreach (var axis in Enum.GetValues(typeof(DuplicationAxis)).Cast<DuplicationAxis>())
            {
                VisibleDuplicationTriggerDictionary.Add(axis, new List<DuplicationTrigger>());
                DuplicationTriggerProximityDictionary.Add(axis, false);
            }

            /*
             * Initialize triggers
             */
            foreach (var duplication_trigger in TriggerHolder.GetComponentsInChildren<DuplicationTrigger>())
            {
                duplication_trigger.Initialize(this);
            }
        }

        //###############################################################

        // -- INQUIRIES

        public bool IsDuplicationAxisVisible(DuplicationAxis axis)
        {
            if(VisibleDuplicationTriggerDictionary[axis].Count > 0)
            {
                return true;
            }

            if (DuplicationTriggerProximityDictionary[axis])
            {
                return true;
            }

            return false;
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
            SetTriggerProximityState();

            foreach (var camera in DuplicationCameras)
            {
                camera.UpdateDuplicationCamera();
            }
        }

        public void OnTriggerStateChanged(DuplicationTrigger trigger, bool is_visible)
        {
            if (is_visible)
            {
                if (!VisibleDuplicationTriggerDictionary[trigger.DuplicationAxis].Contains(trigger))
                {
                    VisibleDuplicationTriggerDictionary[trigger.DuplicationAxis].Add(trigger);
                }
            }
            else
            {
                VisibleDuplicationTriggerDictionary[trigger.DuplicationAxis].Remove(trigger);
            }
        }

        private void SetTriggerProximityState()
        {
            var world_size = GameController.WorldController.WorldSize;
            var player_position = GameController.PlayerController.Transform.position;

            DuplicationTriggerProximityDictionary[DuplicationAxis.Y_Plus_Axis] = false;
            DuplicationTriggerProximityDictionary[DuplicationAxis.Y_Minus_Axis] = false;
            DuplicationTriggerProximityDictionary[DuplicationAxis.Z_Plus_Axis] = false;
            DuplicationTriggerProximityDictionary[DuplicationAxis.Z_Minus_Axis] = false;

            if (player_position.y > world_size.y * 0.5f - 50)
            {
                DuplicationTriggerProximityDictionary[DuplicationAxis.Y_Minus_Axis] = true;
            }
            else if(player_position.y < -world_size.y * 0.5f + 50)
            {
                DuplicationTriggerProximityDictionary[DuplicationAxis.Y_Plus_Axis] = true;
            }

            if (player_position.z > world_size.z * 0.5f - 50)
            {
                DuplicationTriggerProximityDictionary[DuplicationAxis.Z_Minus_Axis] = true;
            }
            else if (player_position.z < -world_size.z * 0.5f + 50)
            {
                DuplicationTriggerProximityDictionary[DuplicationAxis.Z_Plus_Axis] = true;
            }
        }
    }
} // end of namespace