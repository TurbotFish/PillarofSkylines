using UnityEngine;

[ExecuteInEditMode]
public class CameraControlTrigger : MonoBehaviour
{
    public enum CameraControl
    {
        None = 0,
        PointOfInterest = 1,
        AlignWithForwardAxis = 2,
        OverrideCameraTransform = 3
    }

    [HideInInspector, SerializeField] bool displayTarget;

    [Header("Zoom")]
    public bool editZoom;
    [ConditionalHide("editZoom")]
    [Tooltip("Set to -1 to keep default zoom")]
    public float zoomValue = 5;
    [ConditionalHide("editZoom")]
    [Tooltip("Dampening used to zoom in this trigger")]
    public float damp = 1;

    [Header("Parameters")]
    public CameraControl mode;

    [ConditionalHide("displayTarget")]
    public Transform target;

    [ConditionalHide("mode", 2)]
    public bool lookInForwardDirection = false;

    [Space]
    public bool disablePanoramaMode = false;
    [Tooltip("Ignore user input for this amount of seconds")]
    public float ignoreInput = 0;

    new PoS_Camera camera;

    private void OnDrawGizmos()
    {
        if (mode == CameraControl.AlignWithForwardAxis)
        {
            float length = GetComponent<Collider>().bounds.extents.z;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - transform.forward * length, transform.position + transform.forward * length);
        }
    }

    private void OnValidate()
    {
        SetDisplay();
        //tag = "CameraControlTrigger";
        //gameObject.layer = LayerMask.NameToLayer("PickUps");
    }

    /// <summary>
    /// Use to set what shows in the inspector
    /// </summary>
    void SetDisplay()
    {
        displayTarget = mode == CameraControl.PointOfInterest || mode == CameraControl.OverrideCameraTransform;

        if (tag != "CameraControlTrigger")
            tag = "CameraControlTrigger";

        if (gameObject.layer != LayerMask.NameToLayer("PickUps"))
            gameObject.layer = LayerMask.NameToLayer("PickUps");
    }
}
