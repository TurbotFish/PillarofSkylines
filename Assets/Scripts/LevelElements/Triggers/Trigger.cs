using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.Utilities;
using System;
using Game.Model;
using Game.GameControl;
using System.Collections;
using UnityEngine.Serialization;

namespace Game.LevelElements
{
    public abstract class Trigger : UniqueIdOwner, IWorldObject
    {
        //###########################################################

        // -- CONSTANTS

        [FormerlySerializedAs("_triggerState")]
        [SerializeField] private bool IsTriggeredAtStart = false; //the state of the trigger (on/off)

        //DO NOT RENAME
        [SerializeField] private bool toggle; //if true the triggerable object will disregard its own logic an be turned on or off

        //DO NOT RENAME
        [SerializeField] private List<TriggerableObject> targets = new List<TriggerableObject>(); //list of triggerable objects

        [SerializeField, HideInInspector] private List<TriggerableObject> targetsOld = new List<TriggerableObject>();

        //###########################################################

        // -- ATTRIBUTES

        protected TriggerPersistentData PersistentDataObject { get; private set; }
        protected GameController GameController { get; private set; }

        private bool IsInitialized;

        //###########################################################

        // -- INITIALIZATION

        /// <summary>
        /// Initializes the Trigger. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public virtual void Initialize(GameController gameController)
        {
            GameController = gameController;

            PersistentDataObject = GameController.PlayerModel.GetPersistentDataObject<TriggerPersistentData>(UniqueId);

            if (PersistentDataObject == null)
            {
                PersistentDataObject = CreatePersistentDataObject();
                GameController.PlayerModel.AddPersistentDataObject(PersistentDataObject);
            }
            else
            {
                //_triggerState = persistentTrigger.TriggerState;
                //SetTriggerState(persistentTrigger.TriggerState);
                StartCoroutine(LateInitCoroutine());
            }

            IsInitialized = true;
        }

        //###########################################################

        // -- INQUIRIES

        public bool TriggerState { get { return PersistentDataObject.TriggerState; } }

        public bool Toggle { get { return toggle; } }

        public List<TriggerableObject> Targets { get { return new List<TriggerableObject>(targets); } }

        //###########################################################

        // -- OPERATIONS

        /// <summary>
        /// Sets the state of the Trigger. Will send an event to inform all the attached triggerables.
        /// </summary>
        /// <param name="new_trigger_state"></param>
        protected void SetTriggerState(bool new_trigger_state, bool always_execute = false)
        {
            if (!IsInitialized)
            {
                Debug.LogErrorFormat("Trigger {0}: SetTriggerState: Trigger not yet initialized!", this.name);
                return;
            }

            /*
             * if the value does not change we don't do anything
             * "alwaysExecute" skips this check and always executes the trigger even if the value did not change
             */
            if (PersistentDataObject.TriggerState == new_trigger_state && !always_execute)
            {
                return;
            }
            else
            {
                bool old_state = PersistentDataObject.TriggerState;

                PersistentDataObject.TriggerState = new_trigger_state;

                OnTriggerStateChanged(old_state, new_trigger_state);
                EventManager.SendTriggerUpdatedEvent(this, new EventManager.TriggerUpdatedEventArgs(this));
            }
        }

        /// <summary>
        /// Called by SetTriggerState when the state of the trigger changes.
        /// </summary>
        /// <param name="old_state"></param>
        /// <param name="new_state"></param>
        protected abstract void OnTriggerStateChanged(bool old_state, bool new_state);

        /// <summary>
        /// EDITOR: OnValidate
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            //register trigger
            foreach (var target in targets)
            {
                if (target != null && !target.ContainsTrigger(this))
                {
                    target.AddTrigger(this);
                }
            }

            //unregister trigger
            foreach (var target in targetsOld)
            {
                if (target != null && !targets.Contains(target) && target.ContainsTrigger(this))
                {
                    target.RemoveTrigger(this);
                }
            }

            targetsOld = new List<TriggerableObject>(targets);
        }

        /// <summary>
        /// EDITOR: OnDrawGizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = Color.green;

            //targets.RemoveAll(item => item == null); //this helps keeping the target list clean (for example when target objects get deleted)
            foreach (TriggerableObject target in targets)
            {
                if (target != null)
                {
                    Gizmos.DrawLine(Vector3.zero, transform.InverseTransformPoint(target.transform.position));
                }
            }
        }

        /// <summary>
        /// Creates the object containing persitent data for the trigger object. Allows inherited classes to create their own version.
        /// </summary>
        /// <returns></returns>
        protected virtual TriggerPersistentData CreatePersistentDataObject()
        {
            return new TriggerPersistentData(UniqueId)
            {
                TriggerState = IsTriggeredAtStart
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LateInitCoroutine()
        {
            yield return null;

            if (IsTriggeredAtStart != PersistentDataObject.TriggerState)
            {
                //Debug.LogErrorFormat("Trigger {0}: LateInitCoroutine: setting trigger state to {1}", this.name, PersistentDataObject.TriggerState);
                SetTriggerState(PersistentDataObject.TriggerState, true);
            }
        }
    }
} //end of namespace