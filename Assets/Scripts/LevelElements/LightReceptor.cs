using UnityEngine;

public class LightReceptor : Trigger {

    [SerializeField]
    Transform lookAtTarget;
    
    private void OnValidate() {
        if (lookAtTarget) {
            transform.LookAt(lookAtTarget);
            lookAtTarget.LookAt(transform);
        }
    }
}
