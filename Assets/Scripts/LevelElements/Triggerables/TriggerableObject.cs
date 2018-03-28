﻿using UnityEngine;
using System.Collections.Generic;
using System;
using Game.World;
using System.Collections;
using Game.Model;
using Game.Utilities;

namespace Game.LevelElements
{
    /// <summary>
    /// Use this class for objects that can be activated by triggers.
    /// </summary>
    public abstract class TriggerableObject : UniqueIdOwner, IWorldObject
    {
        //###########################################################

        enum TriggerOperator
        {
            None, OneOfThem, AllOfThem
        }

        //###########################################################

        [Header("Triggerable Object")]
        [SerializeField]
        protected bool triggered;

        private void OnDestroy()
        {
            EventManager.TriggerUpdatedEvent -= OnTriggerUpdated;
        }

#if UNITY_EDITOR
        [SerializeField]
        private List<Trigger> triggers = new List<Trigger>(); //list of Trigger objects
#endif

        [SerializeField]
        private TriggerOperator triggerWith = TriggerOperator.AllOfThem;

        [SerializeField]
        private bool definitiveActivation;

        [HideInInspector]
        [SerializeField]
        private List<string> triggerIds = new List<string>(); //list with the Id's of the Trigger objects

        private PlayerModel model;

        private PersistentTriggerable persistentTriggerable;

        private bool isCopy;

        //###########################################################

        #region properties

        public bool Triggered { get { return triggered; } }

        protected PersistentTriggerable PersistentDataObject { get { return persistentTriggerable; } }

        #endregion properties

        //###########################################################

        #region abstract

        protected abstract void Activate();
        protected abstract void Deactivate();

        #endregion abstract

        //###########################################################

        #region public methods

        /// <summary>
        /// Initializes the TriggerableObject. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public virtual void Initialize(GameControl.IGameControllerBase gameController, bool isCopy)
        {
            //
            this.isCopy = isCopy;
            model = gameController.PlayerModel;

            //
            persistentTriggerable = model.GetPersistentDataObject<PersistentTriggerable>(UniqueId);

            if (persistentTriggerable == null)
            {
                persistentTriggerable = CreatePersistentObject();
                model.AddPersistentDataObject(persistentTriggerable);
            }
            else
            {
                SetTriggered(persistentTriggerable.Triggered, true);
            }

            //
            EventManager.TriggerUpdatedEvent += OnTriggerUpdated;
        }

        /// <summary>
        /// Sets the state of the triggerable object. Has no effect if the state does not change.
        /// </summary>
        /// <param name="triggered"></param>
        public virtual void SetTriggered(bool triggered, bool initializing = false)
        {
            if (triggered == this.triggered)
            {
                return;
            }

            this.triggered = triggered;
            persistentTriggerable.Triggered = triggered;

            if (triggered)
            {
                Activate();
            }
            else if (!definitiveActivation)
            {
                Deactivate();
            }
        }

#if UNITY_EDITOR
        public bool ContainsTrigger(Trigger trigger)
        {
            return triggers.Contains(trigger);
        }
#endif

#if UNITY_EDITOR
        public void AddTrigger(Trigger trigger)
        {
            if (!triggers.Contains(trigger))
            {
                triggers.Add(trigger);
                triggerIds.Add(trigger.UniqueId);
            }
        }
#endif

#if UNITY_EDITOR
        public void RemoveTrigger(Trigger trigger)
        {
            triggers.Remove(trigger);
            triggerIds.Remove(trigger.UniqueId);
        }
#endif

        #endregion public methods

        //###########################################################

        #region monobehaviour methods

        private void OnDestroy()
        {
            EventManager.TriggerUpdatedEvent -= OnTriggerUpdated;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            //rebuild trigger id list
            triggerIds.Clear();
            var invalidTriggers = new List<Trigger>();

            foreach (var trigger in triggers)
            {
                if (trigger != null && !trigger.Targets.Contains(this))
                {
                    invalidTriggers.Add(trigger);
                }
            }

            foreach (var trigger in triggers)
            {
                if (trigger != null && !triggerIds.Contains(trigger.UniqueId))
                {
                    triggerIds.Add(trigger.UniqueId);
                }
            }
        }
#endif

        #endregion monobehaviour methods

        //###########################################################

        #region protected methods

        /// <summary>
        /// Creates the object containing persitent data for the triggerable object. Allows inherited classes to create their own version.
        /// </summary>
        /// <returns></returns>
        protected virtual PersistentTriggerable CreatePersistentObject()
        {
            return new PersistentTriggerable(this);
        }

        #endregion protected methods

        //###########################################################

        #region private methods

        private void UpdateState(bool toggle = false)
        {
            if (toggle)
            {
                SetTriggered(triggered ^ true);
            }
            else
            {
                SetTriggered(CheckTriggers());
            }
        }

        private bool CheckTriggers()
        {
            var persistentTriggers = new List<PersistentTrigger>();
            foreach (var triggerId in triggerIds)
            {
                persistentTriggers.Add(model.GetPersistentDataObject<PersistentTrigger>(triggerId));
            }

            switch (triggerWith)
            {
                case TriggerOperator.AllOfThem: //if one trigger is not active, the check fails
                    foreach (var persistentTrigger in persistentTriggers)
                    {
                        if (!persistentTrigger.TriggerState)
                        {
                            return false;
                        }
                    }

                    return true;

                case TriggerOperator.OneOfThem: //if one trigger is active, the check succeeds
                    foreach (var persistentTrigger in persistentTriggers)
                    {
                        if (persistentTrigger.TriggerState)
                        {
                            return true;
                        }
                    }

                    return false;

                case TriggerOperator.None:  //if one trigger is active the check fails
                    foreach (var persistentTrigger in persistentTriggers)
                    {
                        if (persistentTrigger.TriggerState)
                        {
                            return false;
                        }
                    }

                    return true;

                

                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Event handler that is called when a Trigger is toggled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTriggerUpdated(object sender, Utilities.EventManager.TriggerUpdatedEventArgs args)
        {
            if (triggerIds.Contains(args.TriggerId))
            {
                Debug.Log("xxx: " + args.TriggerId);

                UpdateState(args.Trigger.Toggle);
            }
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace