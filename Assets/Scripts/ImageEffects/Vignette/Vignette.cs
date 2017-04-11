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
    
    [SerializeField, ColorUsage(false), Tooltip("The Color of the Vignetting effect.")]
    Color _color = Color.black;
    /// <summary>
    /// The Color of the Vignetting effect.
    /// </summary>
    public Color color {
        get { return _color; }
        set { _color = value; }
    }

    [SerializeField, Tooltip("Intensity of the Vignette from the edges of the screen.")]
    float _strength = 2;
    /// <summary>
    /// Intensity of the Vignette from the edges of the screen.
    /// </summary>
    public float strength {
        get { return _strength; }
        set { _strength = value; }
    }

    #endregion

    #region Private Properties

    [SerializeField] Shader _shader;
    Material _material;

    #endregion

    #region MonoBehaviour Functions

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        var cam = GetComponent<Camera>();
        _material.SetVector("_Aspect", new Vector2(cam.aspect, 1));
        _material.SetFloat("_Falloff", _falloff);
        _material.SetFloat("_Strength", _strength);
        _material.SetColor("_Color", _color);

        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
