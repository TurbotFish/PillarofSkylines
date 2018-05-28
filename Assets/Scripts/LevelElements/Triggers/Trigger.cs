using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.Utilities;
using System;
using Game.Model;
using Game.GameControl;

namespace Game.LevelElements
{
    public abstract class Trigger : UniqueIdOwner, IWorldObject
    {
        //###########################################################

        //DO NOT RENAME
        [SerializeField]
        bool _triggerState = false; //the state of the trigger (on/off)

        //DO NOT RENAME
        [SerializeField]
        private bool toggle; //if true the triggerable object will disregard its own logic an be turned on or off

#if UNITY_EDITOR
        //DO NOT RENAME
        [SerializeField]
        private List<TriggerableObject> targets = new List<TriggerableObject>(); //list of triggerable objects
#endif

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private List<TriggerableObject> targetsOld = new List<TriggerableObject>();
#endif

        private PlayerModel model;

        private TriggerPersistentData persistentTrigger;

        private bool isInitialized;

        //###########################################################

        #region properties

        public bool TriggerState { get { return _triggerState; } }

        public bool Toggle { get { return toggle; } }

#if UNITY_EDITOR
        public List<TriggerableObject> Targets { get { return new List<TriggerableObject>(targets); } }
#endif

        protected TriggerPersistentData PersistentDataObject { get { return persistentTrigger; } }

        #endregion properties

        //###########################################################

        #region public methods

        /// <summary>
        /// Initializes the Trigger. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public virtual void Initialize(GameController gameController)
        {
            model = gameController.PlayerModel;

            persistentTrigger = model.GetPersistentDataObject<TriggerPersistentData>(UniqueId);

            if (persistentTrigger == null)
            {
                persistentTrigger = CreatePersistentDataObject();
                model.AddPersistentDataObject(persistentTrigger);
            }
            else
            {
                _triggerState = persistentTrigger.TriggerState;
            }

            isInitialized = true;
        }

        /// <summary>
        /// Sets the state of the Trigger. Will send an event to inform all the attached triggerables.
        /// </summary>
        /// <param name="triggerState"></param>
        protected void SetTriggerState(bool triggerState, bool alwaysExecute = false)
        {
            if (!isInitialized)
            {
                return;
            }
            if (_triggerState == triggerState && !alwaysExecute) //if the value does not change we don't do anything
            { // "alwaysExecute" skips this check and always executes the trigger even if the value did not change
                return;
            }
            else
            {
                _triggerState = triggerState;

                if (persistentTrigger != null)
                {
                    persistentTrigger.TriggerState = _triggerState;
                }

                EventManager.SendTriggerUpdatedEvent(this, new EventManager.TriggerUpdatedEventArgs(this));
            }
        }

        #endregion public methods

        //###########################################################

        #region monobehaviour methods

#if UNITY_EDITOR
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
#endif

#if UNITY_EDITOR
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
#endif

        #endregion monobehaviour methods

        //###########################################################

        #region protected methods

        /// <summary>
        /// Creates the object containing persitent data for the trigger object. Allows inherited classes to create their own version.
        /// </summary>
        /// <returns></returns>
        protected virtual TriggerPersistentData CreatePersistentDataObject()
        {
            return new TriggerPersistentData(this);
        }

        #endregion protected methods

        //###########################################################
    }
} //end of namespace