using UnityEngine;
using System.Collections.Generic;
using System;
using Game.World;
using Game.World.ChunkSystem;
using Game.LevelElements;
using System.Collections;

namespace Game.LevelElements
{
    /// <summary>
    /// Use this class for objects that can be activated by triggers.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class TriggerableObject : MonoBehaviour, IWorldObjectInitialization
    {
        //###########################################################

        enum TriggerOperator
        {
            None, OneOfThem, AllOfThem
        }

        //###########################################################

#if UNITY_EDITOR
        [SerializeField]
        //[HideInInspector]
        private int instanceId = 0; //used in editor to detect duplication
#endif

        [SerializeField]
        //[HideInInspector]
        private string id;

        [Header("Triggerable Object")]
        [SerializeField]
        protected bool triggered;

#if UNITY_EDITOR
        [SerializeField]
        private List<Trigger> triggers; //list of Trigger objects
#endif

        [SerializeField]
        private TriggerOperator triggerWith = TriggerOperator.AllOfThem;

        [SerializeField]
        private bool definitiveActivation;

        [SerializeField]
        //[HideInInspector]
        private List<string> triggerIds = new List<string>(); //list with the Id's of the Trigger objects

        private PersistentTriggerable persistentTriggerable;
        private bool isCopy;

        //###########################################################

        public string Id { get { return id; } }
        public bool Triggered { get { return triggered; } }

        //###########################################################

        protected abstract void Activate();
        protected abstract void Deactivate();

        //###########################################################

        #region editor methods

#if UNITY_EDITOR

        public bool ContainsTrigger(Trigger trigger)
        {
            return triggers.Contains(trigger);
        }

        public void AddTrigger(Trigger trigger)
        {
            if (!triggers.Contains(trigger))
            {
                triggers.Add(trigger);
                triggerIds.Add(trigger.Id);
            }
        }

        public void RemoveTrigger(Trigger trigger)
        {
            triggers.Remove(trigger);
            triggerIds.Remove(trigger.Id);
        }

#endif

        #endregion editor methods

        //###########################################################

        #region monobehaviour methods

        /// <summary>
        /// EDITOR: sets or resets the id of the TriggerableObject.
        /// </summary>
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }
            else if (instanceId == 0) //first time
            {
                instanceId = GetInstanceID();

                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }

                //filling the list of trigger Id's
                if (triggerIds.Count != triggers.Count)
                {
                    triggerIds.Clear();

                    foreach (var trigger in triggers)
                    {
                        if (trigger == null)
                        {
                            Debug.LogWarning("Triggerable: \"" + gameObject.name + "\" contains null triggers.");
                        }
                        else if (!triggerIds.Contains(trigger.Id))
                        {
                            triggerIds.Add(trigger.Id);
                        }
                    }
                }
            }
            else if (instanceId != GetInstanceID() && GetInstanceID() < 0) //the script has been duplicated
            {
                instanceId = GetInstanceID();

                id = Guid.NewGuid().ToString();

                //resetting things
                triggers.Clear();
                triggerIds.Clear();
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!isCopy)
            {
                persistentTriggerable.Triggered = triggered;
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!isCopy)
            {
                OnDestroy();
            }
        }

        #endregion monobehaviour methods

        //###########################################################

        #region public methods

        /// <summary>
        /// Initializes the TriggerableObject. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public void Initialize(GameControl.IGameControllerBase gameController, bool isCopy)
        {
            //Debug.Log("triggerable " + gameObject.activeInHierarchy);

            //
            this.isCopy = isCopy;

            //
            var model = gameController.PlayerModel;
            persistentTriggerable = model.GetPersistentTriggerable(id);

            if (persistentTriggerable == null)
            {
                persistentTriggerable = new PersistentTriggerable(this);
                model.AddPersistentTriggerable(persistentTriggerable);
            }
            else
            {
                triggered = persistentTriggerable.Triggered;
            }

            StartCoroutine(OnTriggeredChangedCR());

            //
            Game.Utilities.EventManager.TriggerUpdatedEvent += OnTriggerUpdated;
        }

        #endregion public methods

        //###########################################################

        #region private methods

        // Have a ToggleState() to toggle without needing to check states and all since toggle is immediate
        private void UpdateState(bool toggle = false)
        {
            if (toggle)
            {
                triggered ^= true;

                OnTriggeredChanged();
            }
            else
            {
                bool needsUpdate = triggered; //get previous state
                triggered = CheckTriggers();  //check triggers
                needsUpdate ^= triggered;     //see if state has changed

                if (needsUpdate)
                {
                    OnTriggeredChanged();
                }
            }
        }

        private bool CheckTriggers()
        {
            switch (triggerWith)
            {
                case TriggerOperator.AllOfThem:
                    foreach (Trigger trigger in triggers)
                        if (!trigger.TriggerState) return false;
                    return true;

                case TriggerOperator.OneOfThem:
                    foreach (Trigger trigger in triggers)
                        if (trigger.TriggerState) return true;
                    return false;

                case TriggerOperator.None:
                    foreach (Trigger trigger in triggers)
                        if (trigger.TriggerState) return false;
                    return true;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// This should be called when the value of "triggered" is changed.
        /// </summary>
        private void OnTriggeredChanged()
        {
            if (triggered)
            {
                Activate();
            }
            else if (!definitiveActivation)
            {
                Deactivate();
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

        private IEnumerator OnTriggeredChangedCR()
        {
            yield return null;
            yield return null;

            OnTriggeredChanged();
        }

        #endregion private methods

        //###########################################################
    }
} //end of namespace