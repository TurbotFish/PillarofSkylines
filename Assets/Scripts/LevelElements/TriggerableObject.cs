using UnityEngine;
using System.Collections.Generic;
using System;
using Game.World;
using Game.World.ChunkSystem;

/// <summary>
/// Use this class for objects that can be activated by triggers.
/// </summary>
public abstract class TriggerableObject : MonoBehaviour, IWorldObjectInitialization
{
    //###########################################################

    enum TriggerOperator
    {
        None, OneOfThem, AllOfThem
    }

    //###########################################################

    [SerializeField]
    [HideInInspector]
    private string id;

    [Header("Triggerable Object")]
    public bool triggered;

    public List<Trigger> triggers;

    [SerializeField]
    private TriggerOperator triggerWith = TriggerOperator.AllOfThem;

    [SerializeField]
    private bool definitiveActivation;

    //###########################################################

    protected abstract void Activate();
    protected abstract void Deactivate();

    //###########################################################

    public virtual void Initialize(WorldController worldController, bool isCopy)
    {

    }

    //###########################################################

    // Have a ToggleState() to toggle without needing to check states and all since toggle is immediate
    public void UpdateState(bool toggle = false)
    {
        if (toggle)
        {
            triggered ^= true;
            if (triggered) Activate();
            else Deactivate();
        }
        else
        {
            bool needsUpdate = triggered; //get previous state
            triggered = CheckTriggers();  //check triggers
            needsUpdate ^= triggered;     //see if state has changed
            if (needsUpdate)
            {
                if (triggered) Activate();
                else if (!definitiveActivation) Deactivate();
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

    /*private void OnValidate() {
        foreach(Trigger trigger in triggers) {
            if(!trigger.targets.Contains(this)) {
                trigger.targets.Add(this);
            }
        }
    }*/

    //###########################################################
}
