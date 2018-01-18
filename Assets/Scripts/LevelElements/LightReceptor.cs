using UnityEngine;

public class LightReceptor : Trigger {

    [SerializeField]
    Transform lookAtTarget;
    Renderer rend;
    [SerializeField] Material on;
    [SerializeField] Material off;

    private void OnValidate() {
        rend = GetComponent<Renderer>();
        if (lookAtTarget) {
            transform.LookAt(lookAtTarget);
            lookAtTarget.LookAt(transform);
        }
    }

    private void Start() {
        rend = GetComponent<Renderer>();
        TriggerState = false;
        rend.sharedMaterial = off;
    }
    
    public void Toggle(bool newState, bool inverse) {
        TriggerState = newState;
        if (TriggerState == inverse)
            rend.sharedMaterial = on;
        else
            rend.sharedMaterial = off;
    }
}
