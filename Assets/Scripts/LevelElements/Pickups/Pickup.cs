using Game.Cutscene;
using Game.GameControl;
using Game.Model;
using Game.Utilities;
using System;
using System.Collections;
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

        [Header("Pickup - Interaction")]
        [SerializeField] private bool _RequiresInput = true;
        [SerializeField] private bool _ShowPickupMessage = true;

        [Header("Pickup - Cutscene")]
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
            if (IsInitialized)
            {
                OnPickupEnabled();
            }
        }

        private void OnDisable()
        {
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
        public abstract Sprite OnPickedUpIcon { get; }

        public Transform Transform { get { return transform; } }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        /// <returns></returns>
        public bool IsInteractable()
        {
            return (!PersistentData.IsPickedUp && _RequiresInput);
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
            if (!_RequiresInput)
            {
                OnInteraction();
            }
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
            if (_RequiresInput)
            {
                string message = "[X]: Accept " + PickupName;
                GameController.UiController.Hud.ShowHelpMessage(message, UniqueId);
            }
        }

        /// <summary>
        /// Implemented from IInteractable.
        /// </summary>
        public void OnHoverEnd()
        {
            if (_RequiresInput)
            {
                GameController.UiController.Hud.HideHelpMessage(UniqueId);
            }
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
            collider.enabled = false;

            /*
             * Animator
             */
            if (tombAnimator != null)
            {
                tombAnimator.SetTombState(true, true, false, OnPickupAnimationDone);
            }
            else
            {
                OnPickupAnimationDone();
            }

            /*
             * Cutscene
             */
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

        /// <summary>
        /// Called when the pickup animation finishes.
        /// </summary>
        private void OnPickupAnimationDone()
        {
            // give content to player
            OnPickedUp();

            /*
             * Announcment
             */
            if (_ShowPickupMessage)
            {
                GameController.UiController.Hud.ShowAnnouncmentMessage(OnPickedUpMessage, OnPickedUpDescription, 4, OnPickedUpIcon);
            }

            // inform everyone
            var pickupCollectedEventArgs = new EventManager.PickupCollectedEventArgs(UniqueId);
            EventManager.SendPickupCollectedEvent(this, pickupCollectedEventArgs);
        }
    }
} // end of namespace