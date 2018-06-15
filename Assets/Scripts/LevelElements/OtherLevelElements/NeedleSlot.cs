﻿using Game.EchoSystem;
using Game.GameControl;
using Game.Model;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class NeedleSlot : PersistentLevelElement<NeedleSlotPersistentData>, IInteractable, IWaypoint
    {
        //########################################################################

        [Header("NeedleSlot")]
        [SerializeField] private bool hasNeedleAtStart;
        [SerializeField] private GameObject needleGameObject;

        [Header("Waypoint")]
        [SerializeField] private bool useCameraAngle;
        [SerializeField] private float cameraAngle;

        //########################################################################

        // -- INITIALIZATION

        public override void Initialize(GameController gameController)
        {
            base.Initialize(gameController);

            needleGameObject.SetActive(PersistentData.ContainsNeedle);
        }

        //########################################################################

        // -- INQUIRIES

        public Transform Transform { get { return transform; } }

        public Vector3 Position { get { return Transform.position; } }

        public bool IsInteractable()
        {
            return ((PersistentData.ContainsNeedle || GameController.PlayerModel.PlayerHasNeedle) && GameController.PlayerController.CharController.isHandlingInput);
        }

        public bool UseCameraAngle { get { return useCameraAngle; } }
        public float CameraAngle { get { return cameraAngle; } }

        //########################################################################

        // -- OPERATIONS

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            string message = GameController.PlayerModel.PlayerHasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle";
            GameController.UiController.Hud.ShowHelpMessage(message, UniqueId);
        }

        public void OnHoverEnd()
        {
            GameController.UiController.Hud.HideHelpMessage(UniqueId);
        }

        public void OnInteraction()
        {
            if (!IsInteractable())
            {
                return;
            }

            if (!GameController.PlayerModel.PlayerHasNeedle)
            {
                StartCoroutine(TakeNeedleAnimation());
            }
            else
            {
                EndInteraction();
            }

        }

        void EndInteraction()
        {

            /*
             * transfering needle
             */
            needleGameObject.SetActive(GameController.PlayerModel.PlayerHasNeedle);

            GameController.PlayerModel.PlayerHasNeedle ^= true;
            PersistentData.ContainsNeedle = !GameController.PlayerModel.PlayerHasNeedle;

            /*
             * "reacting" to the transfer
             */
            if (GameController.PlayerModel.PlayerHasNeedle)
            {
                GameController.EchoManager.SetWaypoint(this);
            }
            else
            {
                GameController.EchoManager.RemoveWaypoint();    // Removing ALL the waypoints.
            }

            EventManager.SendEclipseEvent(this, new EventManager.EclipseEventArgs(GameController.PlayerModel.PlayerHasNeedle)); // This will activate the player needle.

            string message = GameController.PlayerModel.PlayerHasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle";
            GameController.UiController.Hud.ShowHelpMessage(message, UniqueId);
        }

        IEnumerator TakeNeedleAnimation()
        {
            
            /*
             * transfering needle
             */
            needleGameObject.SetActive(GameController.PlayerModel.PlayerHasNeedle);

            GameController.PlayerModel.PlayerHasNeedle ^= true;
            PersistentData.ContainsNeedle = !GameController.PlayerModel.PlayerHasNeedle;

            EventManager.SendEclipseEvent(this, new EventManager.EclipseEventArgs(GameController.PlayerModel.PlayerHasNeedle)); // This will activate the player needle.

            GameController.PlayerController.CharController.SetHandlingInput(false);
            EventManager.TeleportPlayerEventArgs args = new EventManager.TeleportPlayerEventArgs(GameController.PlayerController.transform.position, transform.position + transform.up + Vector3.down, Quaternion.identity);
            GameController.PlayerController.CharController.SetVelocity(new Vector3(0, 0, 0), false);
            EventManager.SendTeleportPlayerEvent(this, args);
            GameController.PlayerController.CharController.animator.SetTrigger("Take needle");
            yield return new WaitForSeconds(1.8f);

            GameController.PlayerController.CharController.SetHandlingInput(true);

            /*
             * "reacting" to the transfer
             */
            if (GameController.PlayerModel.PlayerHasNeedle)
            {
                GameController.EchoManager.SetWaypoint(this);
            }
            else
            {
                GameController.EchoManager.RemoveWaypoint();    // Removing ALL the waypoints.
            }


            string message = GameController.PlayerModel.PlayerHasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle";
            GameController.UiController.Hud.ShowHelpMessage(message, UniqueId);

        }

        /// <summary>
        /// Called by the echo manager when the waypoint is removed or replaced.
        /// </summary>
        void IWaypoint.OnWaypointRemoved()
        {
        }

        protected override PersistentData CreatePersistentDataObject()
        {
            return new NeedleSlotPersistentData(UniqueId, hasNeedleAtStart);
        }

        //########################################################################
    }
} // end of namespace