using UnityEngine;

public class HomePortalCamera : MonoBehaviour {

    [SerializeField] Shader shader;

    public Transform anchorPoint, worldAnchorPoint;
    [Space]
    [SerializeField] Renderer portalRenderer;
    [SerializeField] GameObject otherPortal;

    [HideInInspector, SerializeField] RenderTexture texture;
    
    Transform my, worldCamera;
    Material mat;
    Camera cam, trueCam;
    Vector3 negDistance;

    private void Start() {
        my = transform;
        cam = GetComponent<Camera>();

        texture = new RenderTexture(Screen.width, Screen.height, 16);
        texture.Create();
        cam.forceIntoRenderTexture = true;
        cam.targetTexture = texture;

        mat = new Material(shader);
        portalRenderer.sharedMaterial = mat;
    }
    
    private void Update() {
        if (!worldCamera)
            FindWorldCamera();

        cam.fieldOfView = trueCam.fieldOfView;
        
        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(worldAnchorPoint.rotation, anchorPoint.rotation);

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * worldCamera.forward;
        my.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

        // distance = distance(worldcamera, worldAnchor)
        // negdistance = (0, 0, -distance)
        // position = rotation * negdistance + targetPoint

        float distance = Vector3.Distance(worldCamera.position, worldAnchorPoint.position);
        negDistance.z = -distance;
        my.position = my.rotation * negDistance + anchorPoint.position + 2 * Vector3.up; // rajoute 2 en y

        //Vector3 camPos = worldAnchorPoint.position - worldCamera.localPosition;
        //my.position = anchorPoint.position - camPos;
        
        mat.mainTexture = texture;
    }

    private void OnEnable() {
        otherPortal.transform.position = anchorPoint.transform.position;
    }

    //

    void FindWorldCamera() {
        worldCamera = FindObjectOfType<PoS_Camera>().transform;
        trueCam = worldCamera.GetComponent<Camera>();
    }
}
