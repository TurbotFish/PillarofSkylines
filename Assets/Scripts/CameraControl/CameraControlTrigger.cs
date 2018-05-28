using Game.GameControl;
using Game.LevelElements;
using Game.World;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour, IInteractable, IWorldObject
{
    public enum CameraControl
    {
        None = 0,
        PointOfInterest = 1,
        AlignWithForwardAxis = 2,
        OverrideCameraTransform = 3
    }

    //########################################################################

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

    private GameController gameController;

    //########################################################################

    public void Initialize(GameController gameController)
    {
        this.gameController = gameController;
    }

    //########################################################################

    public Transform Transform { get { return transform; } }

    public bool IsInteractable()
    {
        return false;
    }

    //########################################################################

    public void OnPlayerEnter()
    {
        if (gameController != null)
        {
            gameController.CameraController.PoS_Camera.EnterTrigger(this);
        }
    }

    public void OnPlayerExit()
    {
        if (gameController != null)
        {
            gameController.CameraController.PoS_Camera.ExitTrigger(this);
        }
    }

    public void OnHoverBegin()
    {
    }

    public void OnHoverEnd()
    {
    }

    public void OnInteraction()
    {
    }

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
    private void SetDisplay()
    {
        displayTarget = mode == CameraControl.PointOfInterest || mode == CameraControl.OverrideCameraTransform;

        if (tag != "CameraControlTrigger")
            tag = "CameraControlTrigger";

        if (gameObject.layer != LayerMask.NameToLayer("PickUps"))
            gameObject.layer = LayerMask.NameToLayer("PickUps");
    }

    //########################################################################
}
