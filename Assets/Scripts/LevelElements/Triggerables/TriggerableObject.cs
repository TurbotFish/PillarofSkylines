using UnityEngine;
using System.Collections.Generic;
using System;
using Game.World;
using Game.Model;
using Game.Utilities;
using Game.GameControl;
using System.Collections;

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

        // -- CONSTANTS

        [Header("Triggerable Object")]
        [SerializeField] private bool triggered;
        [SerializeField] private List<Trigger> triggers = new List<Trigger>(); //list of Trigger objects => SHOULD NOT BE USED AT RUNTIME!!!!
        [SerializeField] private TriggerOperator triggerWith = TriggerOperator.AllOfThem;
        [SerializeField] private bool definitiveActivation;

        [SerializeField, HideInInspector] private List<string> triggerIds = new List<string>(); //list with the Id's of the Trigger objects

        //###########################################################

        // -- ATTRIBUTES

        public bool IsInitialized { get; private set; }

        protected TriggerablePersistentData PersistentDataObject { get; private set; }

        private PlayerModel model;

        //###########################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the TriggerableObject. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        public virtual void Initialize(GameController game_controller)
        {
            model = game_controller.PlayerModel;

            PersistentDataObject = model.GetPersistentDataObject<TriggerablePersistentData>(UniqueId);

            if (PersistentDataObject == null)
            {
                PersistentDataObject = CreatePersistentObject();
                model.AddPersistentDataObject(PersistentDataObject);
            }
            else
            {
                if (this is Door && PersistentDataObject.Triggered != Triggered)
                {
                    Debug.LogWarningFormat("TriggerableObject {0}: Initialize: persistentTriggered={1} currentTriggered={2}", this.name, PersistentDataObject.Triggered, Triggered);
                }

                //SetTriggered(PersistentDataObject.Triggered, true);
                StartCoroutine(LateInitCoroutine());
            }

            IsInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if (IsInitialized && PersistentDataObject.Triggered != triggered)
            {
                SetTriggered(PersistentDataObject.Triggered);
            }

            EventManager.TriggerUpdatedEvent += OnTriggerUpdated;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            EventManager.TriggerUpdatedEvent -= OnTriggerUpdated;
        }

        //###########################################################

        // -- INQUIRIES

        public bool Triggered { get { return triggered; } }

        //###########################################################

        // -- OPERATIONS

        /// <summary>
        /// Sets the state of the triggerable object. Has no effect if the state does not change.
        /// </summary>
        /// <param name="new_trigger_state"></param>
        public virtual void SetTriggered(bool new_trigger_state, bool initializing = false)
        {
            if (!IsInitialized)
            {
                Debug.LogErrorFormat("TriggerableObject {0}: SetTriggered: called while not initialized!", this.name);
                return;
            }

            if (new_trigger_state == this.triggered)
            {
                return;
            }
            else
            {
                this.triggered = new_trigger_state;
                PersistentDataObject.Triggered = new_trigger_state;

                if (new_trigger_state)
                {
                    Activate();
                }
                else if (!definitiveActivation)
                {
                    Deactivate();
                }
            }
        }

        public bool ContainsTrigger(Trigger trigger)
        {
            return triggers.Contains(trigger);
        }

        public void AddTrigger(Trigger trigger)
        {
            if (!triggers.Contains(trigger))
            {
                triggers.Add(trigger);
                triggerIds.Add(trigger.UniqueId);
            }
        }

        public void RemoveTrigger(Trigger trigger)
        {
            triggers.Remove(trigger);
            triggerIds.Remove(trigger.UniqueId);
        }

        protected abstract void Activate();
        protected abstract void Deactivate();

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

        /// <summary>
        /// Creates the object containing persitent data for the triggerable object. Allows inherited classes to create their own version.
        /// </summary>
        /// <returns></returns>
        protected virtual TriggerablePersistentData CreatePersistentObject()
        {
            return new TriggerablePersistentData(UniqueId, Triggered);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toggle"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckTriggers()
        {
            var persistentTriggers = new List<TriggerPersistentData>();
            foreach (var triggerId in triggerIds)
            {
                TriggerPersistentData pers = model.GetPersistentDataObject<TriggerPersistentData>(triggerId);
                if (pers != null)
                    persistentTriggers.Add(pers);
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
            if (!IsInitialized)
            {
                return;
            }

            if (triggerIds.Contains(args.TriggerId))
            {
                //Debug.Log("xxx: " + args.TriggerId);

                UpdateState(args.Trigger.Toggle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LateInitCoroutine()
        {
            yield return null;

            if(triggered != PersistentDataObject.Triggered)
            {
                //Debug.LogErrorFormat("TriggerableObject {0}: LateInitCoroutine: setting triggered to {1}", this.name, PersistentDataObject.Triggered);
                SetTriggered(PersistentDataObject.Triggered, true);
            }
        }
    }
} //end of namespace