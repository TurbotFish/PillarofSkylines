using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.Utilities;
using System;
using Game.Model;

namespace Game.LevelElements
{
    [RequireComponent(typeof(UniqueId))]
    public abstract class Trigger : MonoBehaviour, IWorldObject
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

        private UniqueId uniqueId;
        private PersistentTrigger persistentTrigger;

        private bool isCopy;
        private bool isInitialized;

        //###########################################################

        #region properties

        public string UniqueId { get { if (!uniqueId) { uniqueId = GetComponent<UniqueId>(); } return uniqueId.Id; } }

        public bool TriggerState { get { return _triggerState; } }

        public bool Toggle { get { return toggle; } }

#if UNITY_EDITOR
        public List<TriggerableObject> Targets { get { return new List<TriggerableObject>(targets); } }
#endif

        #endregion properties

        //###########################################################

        #region public methods

        /// <summary>
        /// Initializes the Trigger. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public virtual void Initialize(GameControl.IGameControllerBase gameController, bool isCopy)
        {
            model = gameController.PlayerModel;
            this.isCopy = isCopy;

            persistentTrigger = model.GetPersistentDataObject<PersistentTrigger>(UniqueId);

            if (persistentTrigger == null)
            {
                persistentTrigger = new PersistentTrigger(UniqueId, _triggerState);
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
        protected void SetTriggerState(bool triggerState)
        {
            if (!isInitialized)
            {
                return;
            }

            if (_triggerState == triggerState) //if the value does not change we don't do anything
            {
                return;
            }
            else
            {
                _triggerState = triggerState;

                if (!isCopy)
                {
                    if (persistentTrigger != null)
                    {
                        persistentTrigger.TriggerState = _triggerState;
                    }

                    EventManager.SendTriggerUpdatedEvent(this, new EventManager.TriggerUpdatedEventArgs(this));
                }
            }
        }

        #endregion public methods

        //###########################################################

        #region monobehaviour methods

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            //add UniqueId component (this is for updating existing gameObjects)
            if (uniqueId == null && GetComponent<UniqueId>() == null)
            {
                uniqueId = gameObject.AddComponent<UniqueId>();
            }

            //register trigger
            foreach (var target in targets)
            {
                if (target && !target.ContainsTrigger(this))
                {
                    target.AddTrigger(this);
                }
            }

            //unregister trigger
            foreach (var target in targetsOld)
            {
                if (target && !targets.Contains(target) && target.ContainsTrigger(this))
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

            targets.RemoveAll(item => item == null); //this helps keeping the target list clean (for example when target objects get deleted)
            foreach (TriggerableObject target in targets)
            {
                Gizmos.DrawLine(Vector3.zero, transform.InverseTransformPoint(target.transform.position));
            }
        }
#endif

        #endregion monobehaviour methods

        //###########################################################
    }
} //end of namespace