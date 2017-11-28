using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    new PoS_Camera camera;
    
    [Header("Zoom")]
    [SerializeField, Tooltip("Set to -1 to keep default zoom")] float zoomValue = -1;
    [SerializeField, Tooltip("Dampening used to zoom in this trigger")] float damp = 1;

    [Header("Parameters")]
    [SerializeField] Transform pointOfInterest;
    [SerializeField] bool alignWithForwardAxis = false;
    [Space]
    [SerializeField] bool enablePanoramaMode = false;
    
    private void Start() {
        camera = FindObjectOfType<PoS_Camera>(); // TODO: fix that
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            if (zoomValue > 0)
                camera.ZoomAt(zoomValue, damp);
            if (pointOfInterest)
                camera.SetPointOfInterest(pointOfInterest.position);
            if (alignWithForwardAxis)
                camera.SetAxisAlignment(transform.forward);

            camera.enablePanoramaMode = enablePanoramaMode;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            if (zoomValue > 0)
                camera.ResetZoom();
            if (pointOfInterest)
                camera.ClearPointOfInterest(pointOfInterest.position);
            if (alignWithForwardAxis)
                camera.RemoveAxisAlignment(transform.forward);

            camera.enablePanoramaMode = true;
        }
    }

    private void OnDrawGizmos() {
        if (alignWithForwardAxis) {
            float length = GetComponent<Collider>().bounds.extents.z;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - transform.forward * length, transform.position + transform.forward * length);
        }
    }
}
