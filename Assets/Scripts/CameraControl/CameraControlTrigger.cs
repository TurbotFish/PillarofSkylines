using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    new PoS_Camera camera;
    
    public enum CameraControl {
        None = 0,
        PointOfInterest = 1,
        AlignWithForwardAxis = 2,
        OverrideCameraTransform = 3
    }
    [HideInInspector, SerializeField] bool displayTarget;

    [Header("Zoom")]
    [SerializeField] bool editZoom;
    [ConditionalHide("editZoom")]
    [SerializeField, Tooltip("Set to -1 to keep default zoom")] float zoomValue = 5;
    [ConditionalHide("editZoom")]
    [SerializeField, Tooltip("Dampening used to zoom in this trigger")] float damp = 1;
    
    [Header("Parameters")]
    public CameraControl mode;

    [ConditionalHide("displayTarget")]
    [SerializeField] Transform target;
    
    [ConditionalHide("mode", 2)]
    [SerializeField] bool lookInForwardDirection = false;
    
    [Space]
    [SerializeField] bool disablePanoramaMode = false;
    
    private void Start() {
        camera = FindObjectOfType<PoS_Camera>(); // TODO: fix that
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            if (editZoom)
                camera.ZoomAt(zoomValue, damp);

            switch(mode) {
                case CameraControl.PointOfInterest:
                    camera.SetPointOfInterest(target.position);
                    break;
                case CameraControl.AlignWithForwardAxis:
                    camera.SetAxisAlignment(transform.forward, !lookInForwardDirection);
                    break;
                case CameraControl.OverrideCameraTransform:
                    camera.OverrideCamera(target.position, target.eulerAngles, damp);
                    break;
                default: break;
            }
            
            camera.enablePanoramaMode = !disablePanoramaMode;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            if (editZoom)
                camera.ResetZoom();
            
            switch (mode) {
                case CameraControl.PointOfInterest:
                    camera.ClearPointOfInterest(target.position);
                    break;
                case CameraControl.AlignWithForwardAxis:
                    camera.RemoveAxisAlignment(transform.forward);
                    break;
                case CameraControl.OverrideCameraTransform:
                    camera.StopOverride();
                    break;
                default: break;
            }
            
            camera.enablePanoramaMode = true;
        }
    }

    private void OnDrawGizmos() {
        if (mode == CameraControl.AlignWithForwardAxis) {
            float length = GetComponent<Collider>().bounds.extents.z;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - transform.forward * length, transform.position + transform.forward * length);
        }
    }

    private void OnValidate() {
        SetDisplay();
    }

    /// <summary>
    /// Use to set what shows in the inspector
    /// </summary>
    void SetDisplay() {
        displayTarget = mode == CameraControl.PointOfInterest || mode == CameraControl.OverrideCameraTransform;
    }
}
