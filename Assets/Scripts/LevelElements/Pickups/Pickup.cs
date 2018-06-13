using Game.Cutscene;
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
    public abstract class Pickup<T> : PersistentLevelElement<T>, IInteractable where T : PickupPersistentData
    {
        //##################################################################

        // -- CONSTANTS

        [Header("Pickup")]
        [SerializeField] private TombAnimator tombAnimator;
        [SerializeField] private new Collider collider;

        [Header("")]
        [SerializeField] private bool PlayCutsceneOnPickup = false;
        [SerializeField] private CutsceneType Cutscene;
        [SerializeField] private bool HideUiInCutscene = false;

        //##################################################################

        // -- INITIALIZATION

        public override void Initialize(GameController gameController)
        {
            if (IsInitialized)
            {
                return;
            }

            base.Initialize(gameController);

            OnPickupEnabled();
        }

        private void OnEnable()
        {
            EventManager.PickupCollectedEvent += OnPickupCollectedEvent;

            if (IsInitialized)
            {
                OnPickupEnabled();
            }
        }

        private void OnDisable()
        {
            EventManager.PickupCollectedEvent -= OnPickupCollectedEvent;
        }

        //##################################################################

        // -- INQUIRIES

        /// <summary>
        /// Has the Pickup already been picked up?
        /// </summary>
        public bool IsPickedUp { get { return PersistentData.IsPickedUp; } }

        public abstract string PickupName { get; }
        public abstract string OnPickedUpMessage { get; }
        public abstract string OnPickedUpDescription { get; }

        public Transform Transform { get { return transform; } }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        /// <returns></returns>
        public bool IsInteractable()
        {
            return !PersistentData.IsPickedUp;
        }

        //##################################################################

        // -- OPERATIONS

        /// <summary>
        /// Used to give the content of the Pickup to the player. Actual content depends on the specific implementation.
        /// </summary>
        /// <returns></returns>
        protected abstract void OnPickedUp();

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        public void OnPlayerEnter()
        {

        }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        public void OnPlayerExit()
        {

        }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        public void OnHoverBegin()
        {
            string message = "[X]: Accept " + PickupName;
            GameController.UiController.Hud.ShowHelpMessage(message, UniqueId);
        }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        public void OnHoverEnd()
        {
            GameController.UiController.Hud.HideHelpMessage(UniqueId);
        }

        /// <summary>
        /// Called when the player picks up the object.
        /// </summary>
        /// <param name="callback"></param>
        public void OnInteraction()
        {
            if (PersistentData.IsPickedUp)
            {
                return;
            }

            PersistentData.IsPickedUp = true;

            if (tombAnimator != null)
            {
                tombAnimator.SetTombState(true, true, false, OnPickupAnimationDone);
            }
            else
            {
                OnPickupAnimationDone();
            }

            if (PlayCutsceneOnPickup)
            {
                GameController.CutsceneManager.PlayCutscene(Cutscene, HideUiInCutscene);
            }
        }

        /// <summary>
        /// Implemented from PersistentLevelElement.
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
                if (tombAnimator != null)
                {
                    tombAnimator.SetTombState(true, false, false);
                }
                else
                {
                    OnPickupAnimationDone();
                }

                collider.enabled = false;
            }
        }

        /// <summary>
        /// Called when the pickup animation finishes.
        /// </summary>
        private void OnPickupAnimationDone()
        {
            // give content to player
            OnPickedUp();

            // show message on screen
            GameController.UiController.Hud.ShowAnnouncmentMessage(OnPickedUpMessage, OnPickedUpDescription);

            // disable collider
            collider.enabled = false;

            // inform duplicate Pickup's
            var pickupCollectedEventArgs = new EventManager.PickupCollectedEventArgs(UniqueId);
            EventManager.SendPickupCollectedEvent(this, pickupCollectedEventArgs);
        }

        /// <summary>
        /// Called when the object is initialized or activated.
        /// </summary>
        protected virtual void OnPickupEnabled()
        {
            if (PersistentData.IsPickedUp)
            {
                if (tombAnimator != null && !tombAnimator.IsTombActivated)
                {
                    tombAnimator.SetTombState(true, false, true);
                }
            }
        }
    }
} // end of namespace