using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    new PoS_Camera camera;

    [SerializeField] float zoomValue = 3;
    [SerializeField] float damp = 1;
    [SerializeField] bool enablePanoramaMode = false;
    [SerializeField] bool alignWithDirection = true;

    private void Start() {
        camera = FindObjectOfType<PoS_Camera>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            camera.ZoomAt(zoomValue, damp);
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
