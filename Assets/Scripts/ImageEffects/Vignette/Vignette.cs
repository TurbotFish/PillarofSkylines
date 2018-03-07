using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Vignette (Grr)")]
public class Vignette : MonoBehaviour {
    #region Public Properties
    
    [SerializeField, Range(0.0f, 1.0f), Tooltip("Closeness of the Vignette to the center of the screen.")]
    float _falloff = 0.5f;
    /// <summary>
    /// Closeness of the Vignette to the center of the screen.
    /// </summary>
    public float fallOff {
        get { return _falloff; }
        set { _falloff = value; }
    }

    [SerializeField, Range(1f, 10f), Tooltip("Closeness of the Vignette to the center of the screen.")]
    float _power = 2f;
    /// <summary>
    /// Closeness of the Vignette to the center of the screen.
    /// </summary>
    public float power {
        get { return _power; }
        set { _power = value; }
    }

    [SerializeField, ColorUsage(false), Tooltip("The Color of the Vignetting effect.")]
    Color _color = Color.black;
    /// <summary>
    /// The Color of the Vignetting effect.
    /// </summary>
    public Color color {
        get { return _color; }
        set { _color = value; }
    }
    
    [SerializeField]
    Vector2 _scale;
    /// <summary>
    /// Ratio of the Vignette (default is screen ratio).
    /// </summary>
    public Vector2 scale {
        get { return _scale; }
        set { _scale = value; }
    }

    #endregion

    #region Private Properties

    [SerializeField] Shader _shader;
    Material _material;

    [SerializeField]
    bool adaptToScreen;

    #endregion

    #region MonoBehaviour Functions

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        var cam = GetComponent<Camera>();

        Vector2 aspect = adaptToScreen ? Vector2.one : new Vector2(cam.aspect + scale.x, 1 + scale.y);
        _material.SetVector("_Aspect", aspect);

        _material.SetFloat("_Power", _power);
        _material.SetFloat("_Falloff", _falloff);
        _material.SetColor("_Color", _color);

        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
