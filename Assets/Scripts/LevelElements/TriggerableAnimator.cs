using UnityEngine;
using System.Collections;

public class TriggerableAnimator : TriggerableObject {

    [SerializeField] Animator animator;

    [SerializeField] AnimatorComponent[] animTriggers;

    private void OnValidate()
    {
        if (!animator && GetComponent<Animator>())
            animator = GetComponent<Animator>();
    }

    protected override void Activate()
    {
        foreach (AnimatorComponent anim in animTriggers)
        {
            switch (anim.type)
            {
                case AnimatorComponent.AnimComponentType.Bool:
                    animator.SetBool(anim.name, anim.boolValueOn);
                    break;
                case AnimatorComponent.AnimComponentType.Float:
                    animator.SetFloat(anim.name, anim.floatValueOn);
                    break;
                default:
                case AnimatorComponent.AnimComponentType.Trigger:
                    if (!anim.triggerWhenOff)
                        animator.SetTrigger(anim.name);
                    break;
                case AnimatorComponent.AnimComponentType.Integer:
                    animator.SetInteger(anim.name, anim.intValueOn);
                    break;
            }
        }
    }

    protected override void Deactivate()
    {
        foreach (AnimatorComponent anim in animTriggers)
        {
            switch (anim.type)
            {
                case AnimatorComponent.AnimComponentType.Bool:
                    animator.SetBool(anim.name, !anim.boolValueOn);
                    break;
                case AnimatorComponent.AnimComponentType.Float:
                    animator.SetFloat(anim.name, anim.floatValueOff);
                    break;
                default:
                case AnimatorComponent.AnimComponentType.Trigger:
                    if (anim.triggerWhenOff)
                        animator.SetTrigger(anim.name);
                    break;
                case AnimatorComponent.AnimComponentType.Integer:
                    animator.SetInteger(anim.name, anim.intValueOff);
                    break;
            }
        }
    }

}


public struct AnimatorComponent
{
    public enum AnimComponentType
    {
        Bool = 0,
        Float = 1,
        Trigger = 2,
        Integer = 3,
    }

    public AnimComponentType type;

    public string name;

    [ConditionalHide("type", 0)]
    public bool boolValueOn;

    [ConditionalHide("type", 1)]
    public float floatValueOn;
    [ConditionalHide("type", 1)]
    public float floatValueOff;
    
    [ConditionalHide("type", 2)]
    public bool triggerWhenOff;

    [ConditionalHide("type", 3)]
    public int intValueOn;
    [ConditionalHide("type", 3)]
    public int intValueOff;
}