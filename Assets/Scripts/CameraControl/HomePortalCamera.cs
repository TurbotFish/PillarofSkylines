using UnityEngine;

public class HomePortalCamera : MonoBehaviour {

    [SerializeField] Shader shader;

    public Transform anchorPoint, worldAnchorPoint;
    [Space]
    public Renderer portalRenderer;
    public Transform otherPortal;

    [HideInInspector, SerializeField] RenderTexture texture;
    
    Transform my, worldCamera;
    Material mat;
    Camera cam, trueCam;
    Vector3 negDistance;
    Vector3 offset = new Vector3(0, 2, 0);

    private void Start() {
        my = transform;
        cam = GetComponent<Camera>();

        texture = new RenderTexture(Screen.width, Screen.height, 16);
        texture.Create();
        cam.forceIntoRenderTexture = true;
        cam.targetTexture = texture;

        mat = new Material(shader);
        portalRenderer.sharedMaterial = mat;

        otherPortal.parent = null;
        otherPortal.position = anchorPoint.transform.position;

    }
    
    private void Update() {
        if (!worldCamera)
            FindWorldCamera();

        cam.fieldOfView = trueCam.fieldOfView;
        
        //float angularDifferenceBetweenPortalRotations = Quaternion.Angle(anchorPoint.rotation, worldAnchorPoint.rotation);

        //Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        //Vector3 newCameraDirection = portalRotationalDifference * worldCamera.forward;
        //my.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

        my.rotation = Quaternion.identity;

        float distance = Vector3.Distance(worldCamera.position, worldAnchorPoint.position + offset);
        negDistance.z = -distance;
        my.position = my.rotation * negDistance + anchorPoint.position + offset;
        
        mat.mainTexture = texture;
    }

    private void OnEnable()
    {
        otherPortal.gameObject.SetActive(true);
        otherPortal.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        otherPortal.gameObject.SetActive(false);
    }
    
    //

    void FindWorldCamera() {
        worldCamera = FindObjectOfType<PoS_Camera>().transform;
        trueCam = worldCamera.GetComponent<Camera>();
    }
}
