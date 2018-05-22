using Game.EchoSystem;
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

        //########################################################################

        // INITIALIZATION

        public override void Initialize(IGameController gameController)
        {
            base.Initialize(gameController);

            needleGameObject.SetActive(PersistentData.ContainsNeedle);
        }

        //########################################################################

        // INQUIRIES

        public Transform Transform { get { return transform; } }

        public Vector3 Position { get { return Transform.position; } }

        public bool IsInteractable()
        {
            return (PersistentData.ContainsNeedle || GameController.PlayerModel.HasNeedle);
        }

        //########################################################################

        // OPERATIONS

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, GameController.PlayerModel.HasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle");
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnHoverEnd()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(false);
            EventManager.SendShowHudMessageEvent(this, eventArgs);
        }

        public void OnInteraction()
        {
            if (!IsInteractable())
            {
                return;
            }

            /*
             * transfering needle
             */
            needleGameObject.SetActive(GameController.PlayerModel.HasNeedle);

            GameController.PlayerModel.HasNeedle ^= true;
            PersistentData.ContainsNeedle = !GameController.PlayerModel.HasNeedle;

            /*
             * "reacting" to the transfer
             */
            if (GameController.PlayerModel.HasNeedle)
            {
                GameController.EchoManager.SetWaypoint(this);
            }
            else
            {
                GameController.EchoManager.RemoveWaypoint();    // Removing ALL the waypoints.
            }

            EventManager.SendEclipseEvent(this, new EventManager.EclipseEventArgs(GameController.PlayerModel.HasNeedle)); // This will activate the player needle.

            var showHudEventArgs = new EventManager.OnShowHudMessageEventArgs(true, GameController.PlayerModel.HasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle");
            EventManager.SendShowHudMessageEvent(this, showHudEventArgs);
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