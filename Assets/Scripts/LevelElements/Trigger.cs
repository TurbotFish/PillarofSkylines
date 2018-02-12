using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.World.ChunkSystem;
using Game.Player;
using Game.LevelElements;
using Game.Utilities;

public abstract class Trigger : MonoBehaviour, IWorldObjectInitialization
{
    //###########################################################

    [SerializeField]
    [HideInInspector]
    private string id;

    //DO NOT RENAME
    [SerializeField]
    bool _triggerState = false; //the state of the trigger (on/off)

    //DO NOT RENAME
    [SerializeField]
    private bool toggle; //if true the triggerable object will disregard its own logic an be turned on or off

    //DO NOT RENAME
    [SerializeField]
    private List<TriggerableObject> targets; //list of triggerable objects

    [SerializeField]
    [HideInInspector]
    private List<TriggerableObject> targetsOld = new List<TriggerableObject>();

    private PlayerModel model;
    private PersistentTrigger persistentTrigger;
    private bool isCopy;

    //###########################################################

    public string Id { get { return id; } }

    public bool TriggerState
    {
        get { return _triggerState; }
        protected set
        {
            _triggerState = value;

            //if (!isCopy)
            //{
            //    persistentTrigger.TriggerState = _triggerState;
            //    EventManager.SendTriggerUpdatedEvent(this, new EventManager.TriggerUpdatedEventArgs(this));
            //}

            foreach (TriggerableObject target in targets)
            {
                target.UpdateState(toggle);
            }
        }
    }

    public bool Toggle { get { return toggle; } }

    public List<TriggerableObject> Targets { get { return new List<TriggerableObject>(targets); } }

    //###########################################################

    /// <summary>
    /// Initializes the Trigger. Implemented from IWorldObjectInitialization.
    /// </summary>
    /// <param name="worldController"></param>
    /// <param name="isCopy"></param>
    public virtual void Initialize(WorldController worldController, bool isCopy)
    {
        model = worldController.GameController.PlayerModel;
        persistentTrigger = model.GetPersistentTrigger(id);
        this.isCopy = isCopy;

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

    protected virtual void OnDestroy()
    {
        if (!isCopy)
        {
            persistentTrigger.TriggerState = _triggerState;
        }
    }

    private void OnDisable()
    {
        if (!isCopy)
        {
            OnDestroy();
        }
    }

    //###########################################################

    private void OnValidate()
    {
        foreach (TriggerableObject target in targets)
        {
            if (!target.triggers.Contains(this))
            {
                target.triggers.Add(this);
            }
        }

        foreach (var target in targetsOld)
        {
            if (!targets.Contains(target) && target.triggers.Contains(this))
            {
                target.triggers.Remove(this);
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

    //###########################################################
}
