using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/GradientFog (Grr)")]
public class GradientFog : MonoBehaviour {

	#region Properties

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
        set {
			_start = value;
			SetValues();
		}
    }
    public float endDistance {
        get { return _end; }
        set {
			_end = value;
			SetValues();
		}
    }
    public bool drawOverSkybox {
        get { return _drawOverSkybox; }
        set {
			_drawOverSkybox = value;
			SetValues();
		}
    }

    public Gradient gradient;
    Texture2D texGradient;
    int texSize = 128;
    FilterMode texFilterMode = FilterMode.Bilinear;
	
	Vector3[] frustumCorners;
	Vector4[] vectorArray;
	
	public Gradient secondaryGradient;
	[Range(0, 1)]
	public float gradientLerp = 0;

	#endregion

	void OnValidate() {
        GenerateTexture();
		SetValues();
    }

	void SetValues() {
		mat.SetFloat("_FogStart", startDistance);
		mat.SetFloat("_FogEnd", endDistance);
		Shader.SetGlobalFloat("_FogStart", startDistance);
		Shader.SetGlobalFloat("_FogEnd", endDistance);

		if (_drawOverSkybox)
			mat.EnableKeyword("_DRAW_OVER_SKYBOX");
		else {
			mat.DisableKeyword("_DRAW_OVER_SKYBOX");
		}
	}

    void Start() {
        camera = GetComponent<Camera>();
        if (!camera) Debug.LogError("There is no Camera attached to the Gradient Fog");
        if (!shader) Debug.LogError("There is no Shader attached to the Gradient Fog");
        
        camera.depthTextureMode = DepthTextureMode.Depth;
        GenerateTexture();
		SetValues();
	}
    
    public void GenerateTexture() {
        if (mat == null) {
            mat = new Material(shader);
			mat.hideFlags = HideFlags.DontSave;
			frustumCorners = new Vector3[4];
			vectorArray = new Vector4[4];
		}

		texGradient = new Texture2D(texSize, 1);
        texGradient.filterMode = texFilterMode;
        texGradient.wrapMode = TextureWrapMode.Clamp;
        texGradient.SetPixel(0, 1, new Color(1,1,1,0)); //Sets the first pixel as transparent
        
        for (int i = 1; i < texSize; i++) {
			float time = (float)i / (texSize - 1);
			Color value = Color.Lerp(gradient?.Evaluate(time) ?? Color.white, secondaryGradient?.Evaluate(time) ?? Color.white, gradientLerp);
			texGradient.SetPixel(i, 1, value);
		}
        
        texGradient.Apply();
        mat.SetTexture("_Gradient", texGradient);
		Shader.SetGlobalTexture("_GradientFog", texGradient);
	}

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
		camera.CalculateFrustumCorners(
			new Rect(0f, 0f, 1f, 1f),
			camera.farClipPlane,
			camera.stereoActiveEye,
			frustumCorners
		);

		vectorArray[0] = frustumCorners[0];
		vectorArray[1] = frustumCorners[3];
		vectorArray[2] = frustumCorners[1];
		vectorArray[3] = frustumCorners[2];
		mat.SetVectorArray("_FrustumCorners", vectorArray);

		Graphics.Blit(source, destination, mat);
    }
}