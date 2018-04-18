using Game.GameControl;
using Game.Model;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public class NeedleSlot : PersistentLevelElement<NeedleSlotPersistentData>, IInteractable
    {
        //########################################################################

        [Header("NeedleSlot")]
        [SerializeField] private bool hasNeedleAtStart;
        [SerializeField] private GameObject needleGameObject;

        //########################################################################

        #region initialization

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            base.Initialize(gameController, isCopy);

            needleGameObject.SetActive(PersistentData.ContainsNeedle);
        }

        #endregion initialization

        //########################################################################

        #region inquiries

        public Vector3 Position { get { return transform.position; } }

        public bool IsInteractable()
        {
            return (PersistentData.ContainsNeedle || GameController.PlayerModel.hasNeedle);
        }

        #endregion inquiries

        //########################################################################

        #region operations

        public void OnPlayerEnter()
        {
        }

        public void OnPlayerExit()
        {
        }

        public void OnHoverBegin()
        {
            var eventArgs = new EventManager.OnShowHudMessageEventArgs(true, GameController.PlayerModel.hasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle");
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

            needleGameObject.SetActive(GameController.PlayerModel.hasNeedle);

            GameController.PlayerModel.hasNeedle ^= true;
            PersistentData.ContainsNeedle = !GameController.PlayerModel.hasNeedle;

            if (GameController.PlayerModel.hasNeedle)
            {
                EventManager.SendSetWaypointEvent(this, new EventManager.SetWaypointEventArgs("NeedleSlot", transform.position));
            }

            EventManager.SendEclipseEvent(this, new EventManager.EclipseEventArgs(GameController.PlayerModel.hasNeedle)); // This will activate the player needle.

            var showHudEventArgs = new EventManager.OnShowHudMessageEventArgs(true, GameController.PlayerModel.hasNeedle ? "[X]: Plant Needle" : "[X]: Take Needle");
            EventManager.SendShowHudMessageEvent(this, showHudEventArgs);
        }

        protected override PersistentData CreatePersistentDataObject()
        {
            return new NeedleSlotPersistentData(UniqueId, hasNeedleAtStart);
        }

        #endregion operations

        //########################################################################
    }
} // end of namespace