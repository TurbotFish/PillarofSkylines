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

        my.localRotation = worldCamera.localRotation; // TODO: faire qu'on puisse la tourner

        Vector3 camPos = worldAnchorPoint.position - worldCamera.localPosition;
        my.position = anchorPoint.position - camPos;

        mat.mainTexture = texture;
    }

    private void OnEnable() {
        otherPortal.transform.position = worldAnchorPoint.transform.position;
    }

    //

    void FindWorldCamera() {
        worldCamera = FindObjectOfType<PoS_Camera>().transform;
        trueCam = worldCamera.GetComponent<Camera>();
    }
}
