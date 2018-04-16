using Game.GameControl;
using Game.Model;
using Game.Utilities;
using System;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// Definition of generic Pickup. Other classes can use it without needing to specify the type of PersistentData used.
    /// </summary>
    public abstract class Pickup : Pickup<PickupPersistentData> { }

    /// <summary>
    /// Base class for a Pickup. Can be 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Pickup<T> : PersistentLevelElement<T> where T : PickupPersistentData
    {
        //##################################################################

        [Header("Pickup")]
        [SerializeField] private TombAnimator tombAnimator;
        [SerializeField] private new Collider collider;

        //##################################################################

        #region initialization methods

        public override void Initialize(IGameControllerBase gameController, bool isCopy)
        {
            if (IsInitialized)
            {
                return;
            }

            base.Initialize(gameController, isCopy);

            if (PersistentData.IsPickedUp && !tombAnimator.IsTombActivated)
            {
                tombAnimator.SetTombState(true, false, true);
            }
        }

        private void OnEnable()
        {
            EventManager.PickupCollectedEvent += OnPickupCollectedEvent;

            if (IsInitialized)
            {
                if(PersistentData.IsPickedUp && !tombAnimator.IsTombActivated)
                {
                    tombAnimator.SetTombState(true, false, true);
                }
            }
        }

        private void OnDisable()
        {
            EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
        }

        #endregion initialization methods

        //##################################################################

        #region inquiries

        public bool IsPickedUp { get { return PersistentData.IsPickedUp; } }

        public abstract string PickupName { get; }
        public abstract string OnPickedUpMessage { get; }
        public abstract string OnPickedUpDescription { get; }

        #endregion inquiries

        //##################################################################

        #region operations

        /// <summary>
        /// Used to give the content of the Pickup to the player. Actual content depends on the specific implementation.
        /// </summary>
        /// <returns></returns>
        protected abstract void OnPickedUp();

        /// <summary>
        /// Called when the player picks up the object.
        /// </summary>
        /// <param name="callback"></param>
        public void PickupObject()
        {
            if (PersistentData.IsPickedUp)
            {
                return;
            }

            PersistentData.IsPickedUp = true;

            tombAnimator.SetTombState(true, true, false, OnTombAnimatingDone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override PersistentData CreatePersistentDataObject()
        {
            return new PickupPersistentData(UniqueId);
        }

        /// <summary>
        /// Called when another Pickup is collected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPickupCollectedEvent(object sender, EventManager.PickupCollectedEventArgs args)
        {
            if (IsInitialized && (sender as Pickup<T>).UniqueId == UniqueId && !sender.Equals(this) && PersistentData.IsPickedUp)
            {
                tombAnimator.SetTombState(true, false, false);

                collider.enabled = false;
            }
        }

        /// <summary>
        /// Called when the pickup animation finishes.
        /// </summary>
        private void OnTombAnimatingDone()
        {
            // give content to player
            OnPickedUp();

            // show message on screen
            var HUDMessageEventArgs = new EventManager.OnShowHudMessageEventArgs(true, OnPickedUpMessage, UI.eMessageType.Announcement, OnPickedUpDescription, 4);
            EventManager.SendShowHudMessageEvent(this, HUDMessageEventArgs);

            // disable collider
            collider.enabled = false;

            // inform duplicate Pickup's
            var pickupCollectedEventArgs = new EventManager.PickupCollectedEventArgs(UniqueId);
            EventManager.SendPickupCollectedEvent(this, pickupCollectedEventArgs);
        }

        #endregion operations

        //##################################################################
    }
} // end of namespace