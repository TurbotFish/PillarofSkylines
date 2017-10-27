using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    new PoS_Camera camera;

    [SerializeField] float zoomValue = 3;
    [SerializeField] bool enablePanoramaMode = false;

    private void Start() {
        camera = FindObjectOfType<PoS_Camera>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            camera.ZoomAt(zoomValue);
            camera.enablePanoramaMode = enablePanoramaMode;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            camera.ResetZoom();
            camera.enablePanoramaMode = true;
        }
    }
}
