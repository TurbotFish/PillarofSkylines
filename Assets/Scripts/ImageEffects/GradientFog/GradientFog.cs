using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/GradientFog (Grr)")]
public class GradientFog : MonoBehaviour {

    [SerializeField]
    Shader shader;
    Material mat;
    new Camera camera;

    [SerializeField]
    float _start = 10, _end = 500;

    [SerializeField]
    bool _drawOverSkybox = true;

    public float startDistance {
        get { return _start; }
        set { _start = value; }
    }
    public float endDistance {
        get { return _end; }
        set { _end = value; }
    }
    public bool drawOverSkybox {
        get { return _drawOverSkybox; }
        set { _drawOverSkybox = value; }
    }

    public Gradient gradient;
    Texture2D texGradient;
    int texSize = 128;
    FilterMode texFilterMode = FilterMode.Bilinear;
    
    void OnValidate() {
        GenerateTexture();
        mat.SetFloat("_FogStart", startDistance);
        mat.SetFloat("_FogEnd", endDistance);

        if (_drawOverSkybox)
            mat.EnableKeyword("_DRAW_OVER_SKYBOX");
        else {
            mat.DisableKeyword("_DRAW_OVER_SKYBOX");
            if (!camera) Start();
            mat.SetFloat("_FarClipPlane", camera.farClipPlane);
        }
    }
    
    void Start() {
        camera = GetComponent<Camera>();
        if (!camera) Debug.LogError("There is no Camera attached to the Post Process Depth Grayscale");
        if (!shader) Debug.LogError("There is no Shader attached to the Post Process Depth Grayscale");
        
        camera.depthTextureMode = DepthTextureMode.Depth;
        GenerateTexture();
    }
    
    public void GenerateTexture() {
        if (mat == null) {
            mat = new Material(shader);
            mat.hideFlags = HideFlags.DontSave;
        }

        texGradient = new Texture2D(texSize, 1);
        texGradient.filterMode = texFilterMode;
        texGradient.wrapMode = TextureWrapMode.Clamp;
        texGradient.SetPixel(0, 1, new Color(1,1,1,0)); //Sets the first pixel as transparent
        
        for (int i = 1; i < texSize; i++) {
            texGradient.SetPixel(i, 1, gradient.Evaluate((float)i / (texSize - 1)));
        }
        
        texGradient.Apply();
        mat.SetTexture("_Gradient", texGradient);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, mat);
    }
}