using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    new PoS_Camera camera;
    
    [Header("Zoom")]
    [SerializeField, Tooltip("Set to -1 to keep default zoom")] float zoomValue = -1;
    [SerializeField, Tooltip("Dampening used to zoom in this trigger")] float damp = 1;

    [Header("Parameters")]
    [SerializeField] Transform pointOfInterest;
    [SerializeField] bool enablePanoramaMode = false;

    private void Start() {
        camera = FindObjectOfType<PoS_Camera>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            if (zoomValue > 0) {
                camera.ZoomAt(zoomValue, damp);
            }
            if (pointOfInterest) {
                camera.SetPointOfInterest(pointOfInterest.position);
            }

            camera.enablePanoramaMode = enablePanoramaMode;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            camera.ResetZoom();
            camera.ClearPointOfInterest();
            camera.enablePanoramaMode = true;
        }
    }
}
