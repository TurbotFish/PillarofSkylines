using UnityEngine;

public class HomePortalCamera : MonoBehaviour {

    [SerializeField] Shader shader;

    [SerializeField] Transform AnchorPoint, WorldAnchorPoint;
    [Space]
    [SerializeField] Renderer portalRenderer;

    [HideInInspector, SerializeField] RenderTexture texture;

    Vector3 anchorPoint, worldAnchorPoint;
    Transform my, worldCamera;
    Material mat;
    Camera cam;

    private void Start() {
        my = transform;
        anchorPoint = AnchorPoint.position;
        worldAnchorPoint = WorldAnchorPoint.position;

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

        my.localRotation = worldCamera.localRotation;

        Vector3 camPos = worldAnchorPoint - worldCamera.localPosition;
        my.position = anchorPoint - camPos;

        mat.mainTexture = texture;
    }

    //

    void FindWorldCamera() {
        worldCamera = FindObjectOfType<PoS_Camera>().transform;
    }
}
