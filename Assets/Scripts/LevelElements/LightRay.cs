using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LightRay : MonoBehaviour {

    [SerializeField]
    Transform lookAtTarget;

    LightReceptor receptor;
    LineRenderer renderer;
    Transform my;
    private void Start() {
        my = transform;
        renderer = GetComponent<LineRenderer>();
        renderer.useWorldSpace = false;
        renderer.positionCount = 2;
        renderer.SetPosition(0, Vector3.zero);
    }

    private void Update() {

        RaycastHit hit;
        if (Physics.Raycast(my.position, my.forward, out hit, Mathf.Infinity)) {

            renderer.SetPosition(1, my.InverseTransformPoint(hit.point));
            LightReceptor newReceptor = hit.transform.GetComponent<LightReceptor>();

            if (newReceptor) {
                receptor = newReceptor;
                receptor.TriggerState = false;
            } else if (receptor) {
                receptor.TriggerState = true;
            }
        }
        
        if (lookAtTarget) {
            transform.LookAt(lookAtTarget);
            lookAtTarget.LookAt(my);
        }
    }
}
