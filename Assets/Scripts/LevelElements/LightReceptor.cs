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

    private void Awake() {
        rend = GetComponent<Renderer>();
    }

    public void Toggle(bool newState) {
        TriggerState = newState;
        if (TriggerState)
            rend.sharedMaterial = on;

        else
            rend.sharedMaterial = off;
    }


}
