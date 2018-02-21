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
        [HideInInspector]
        [SerializeField]
        private int instanceId; //used in editor to detect duplication
#endif

        [HideInInspector]
        [SerializeField]
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

        [HideInInspector]
        [SerializeField]
        private List<string> triggerIds = new List<string>(); //list with the Id's of the Trigger objects

        private PersistentTriggerable persistentTriggerable;
        private bool isCopy;
        private Player.PlayerModel model;

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

#if UNITY_EDITOR
        private void OnValidate()
        {
            triggerIds.Clear();
            triggers.RemoveAll(item => item == null);

            foreach (var trigger in triggers)
            {
                triggerIds.Add(trigger.Id);
            }
        }
#endif

        protected virtual void Awake()
        {

        }

        /// <summary>
        /// EDITOR: sets or resets the id of the TriggerableObject.
        /// </summary>
        private void Update()
        {
#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                return;
            }
            else if (instanceId == 0) //first time
            {
                //Debug.Log("triggerable: awake: instanceId == 0!");
                instanceId = GetInstanceID();

                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }

                //filling the list of trigger Id's
                if (triggerIds.Count != triggers.Count)
                {
                    triggerIds.Clear();
                    triggers.RemoveAll(item => item == null);

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

                //"save" changes
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
            else if (instanceId != GetInstanceID() /*&& GetInstanceID() < 0*/) //the script has been duplicated
            {
                //Debug.Log("triggerable: awake: instanceId changed!");
                instanceId = GetInstanceID();

                id = Guid.NewGuid().ToString();

                //resetting things
                triggers.Clear();
                triggerIds.Clear();

                //"save" changes
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }

#endif //UNITY_EDITOR
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
        public virtual void Initialize(GameControl.IGameControllerBase gameController, bool isCopy)
        {
            //Debug.Log("triggerable " + gameObject.activeInHierarchy);

            //
            this.isCopy = isCopy;
            model = gameController.PlayerModel;

            //
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
            var persistentTriggers = new List<PersistentTrigger>();
            foreach (var triggerId in triggerIds)
            {
                persistentTriggers.Add(model.GetPersistentTrigger(triggerId));
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

        //private IEnumerator OnTriggeredChangedCR()
        //{
        //    yield return null;
        //    yield return null;

        //    OnTriggeredChanged();
        //}

        #endregion private methods

        //###########################################################
    }
} //end of namespace