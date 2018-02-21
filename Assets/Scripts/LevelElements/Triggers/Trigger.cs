using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.World.ChunkSystem;
using Game.Player;
using Game.LevelElements;
using Game.Utilities;
using System;

namespace Game.LevelElements
{
    [ExecuteInEditMode]
    public abstract class Trigger : MonoBehaviour, IWorldObjectInitialization
    {
        //###########################################################

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private int instanceId = 0; //used in editor to detect duplication
#endif

        [SerializeField]
        [HideInInspector]
        private string id;

        //DO NOT RENAME
        [SerializeField]
        bool _triggerState = false; //the state of the trigger (on/off)

        //DO NOT RENAME
        [SerializeField]
        private bool toggle; //if true the triggerable object will disregard its own logic an be turned on or off

#if UNITY_EDITOR
        //DO NOT RENAME
        [SerializeField]
        private List<TriggerableObject> targets; //list of triggerable objects
#endif

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private List<TriggerableObject> targetsOld = new List<TriggerableObject>();
#endif

        private PlayerModel model;
        private PersistentTrigger persistentTrigger;
        private bool isCopy;

        //###########################################################

        #region properties

        public string Id { get { return id; } }

        public bool TriggerState
        {
            get { return _triggerState; }
            protected set
            {
                if (_triggerState == value) //if the value does not change we don't do anything
                {
                    return;
                }
                else
                {
                    _triggerState = value;

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
        }

        public bool Toggle { get { return toggle; } }

#if UNITY_EDITOR
        public List<TriggerableObject> Targets { get { return new List<TriggerableObject>(targets); } }
#endif

        #endregion properties

        //###########################################################

        #region editor methods

#if UNITY_EDITOR

        private void OnValidate()
        {
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

        protected virtual void OnDrawGizmos()
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = Color.green;

            foreach (TriggerableObject target in targets)
            {
                Gizmos.DrawLine(Vector3.zero, transform.InverseTransformPoint(target.transform.position));
            }
        }

#endif

        #endregion editor methods

        //###########################################################

        #region monobehaviour methods

        /// <summary>
        /// EDITOR: sets or resets the id of the Trigger.
        /// </summary>
        private void Awake()
        {
#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                return;
            }
            else if (instanceId == 0)
            {
                instanceId = GetInstanceID();

                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }

                //"save" changes
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
            else if (instanceId != GetInstanceID() && GetInstanceID() < 0) //the script has been duplicated
            {
                instanceId = GetInstanceID();

                id = Guid.NewGuid().ToString();

                //resetting things
                targets.Clear();
                targetsOld.Clear();

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
                foreach (var triggerable in targets)
                {
                    triggerable.RemoveTrigger(this);
                }

                return;
            }
#endif

            if (!isCopy)
            {
                persistentTrigger.TriggerState = _triggerState;
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
        /// Initializes the Trigger. Implemented from IWorldObjectInitialization.
        /// </summary>
        /// <param name="worldController"></param>
        /// <param name="isCopy"></param>
        public virtual void Initialize(GameControl.IGameControllerBase gameController, bool isCopy)
        {
            //Debug.Log("trigger " + gameObject.activeInHierarchy);

            model = gameController.PlayerModel;
            this.isCopy = isCopy;

            persistentTrigger = model.GetPersistentTrigger(id);

            if (persistentTrigger == null)
            {
                persistentTrigger = new PersistentTrigger(this);
                model.AddPersistentTrigger(persistentTrigger);
            }
            else
            {
                _triggerState = persistentTrigger.TriggerState;
            }
        }

        #endregion public methods

        //###########################################################
    }
} //end of namespace