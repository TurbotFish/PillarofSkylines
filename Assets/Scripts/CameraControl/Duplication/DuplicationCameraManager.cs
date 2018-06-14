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

        

        private List<DuplicationCamera> DuplicationCameras = new List<DuplicationCamera>();
        private Dictionary<DuplicationAxis, List<DuplicationTrigger>> DuplicationTriggerDictionary = new Dictionary<DuplicationAxis, List<DuplicationTrigger>>();

        //###############################################################

        // -- INITIALIZATION

        public void Initialize(GameController gameController)
        {
            MainCamera = gameController.CameraController.PoS_Camera.CameraComponent;
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
             * Initialize triggers
             */
            foreach(var axis in Enum.GetValues(typeof(DuplicationAxis)).Cast<DuplicationAxis>())
            {
                DuplicationTriggerDictionary.Add(axis, new List<DuplicationTrigger>());
            }

            foreach(var duplication_trigger in TriggerHolder.GetComponentsInChildren<DuplicationTrigger>())
            {
                duplication_trigger.Initialize(this);
                DuplicationTriggerDictionary[duplication_trigger.DuplicationAxis].Add(duplication_trigger);
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

        public void OnTriggerStateChanged(DuplicationTrigger trigger, bool is_visible)
        {
            if (is_visible)
            {
                if (!DuplicationTriggerDictionary[trigger.DuplicationAxis].Contains(trigger))
                {
                    DuplicationTriggerDictionary[trigger.DuplicationAxis].Add(trigger);
                }
            }
            else
            {
                DuplicationTriggerDictionary[trigger.DuplicationAxis].Remove(trigger);
            }
        }
    }
} // end of namespace