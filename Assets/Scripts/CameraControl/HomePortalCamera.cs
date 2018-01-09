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

    private void Start() {
        my = transform;
        anchorPoint = AnchorPoint.position;
        worldAnchorPoint = WorldAnchorPoint.position;

        texture = new RenderTexture(1920, 1080, 0);
        texture.Create();
        GetComponent<Camera>().forceIntoRenderTexture = true;
        GetComponent<Camera>().targetTexture = texture;

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
