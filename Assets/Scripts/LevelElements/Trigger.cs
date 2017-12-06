using UnityEngine;
using System.Collections.Generic;

public abstract class Trigger : MonoBehaviour {

    [SerializeField]
    bool _triggerState = false;
    public bool TriggerState {
        get { return _triggerState; }
        set {
            _triggerState = value;
            foreach(TriggerableObject target in targets) {
                target.UpdateState(toggle);
            }
        }
    }
    protected bool toggle;

    public List<TriggerableObject> targets;

    private void OnValidate() {
        TriggerState = _triggerState;
        foreach(TriggerableObject target in targets) {
            if (!target.triggers.Contains(this)) {
                target.triggers.Add(this);
            }
        }
    }

    protected virtual void OnDrawGizmos() {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = Color.green;

        foreach (TriggerableObject target in targets) {
            Gizmos.DrawLine(Vector3.zero, transform.InverseTransformPoint(target.transform.position));
        }
    }
}
