using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/RadialBlur (Grr)")]
public class RadialBlur : MonoBehaviour {
    #region Public Properties

    [SerializeField, Range(0.0f, 1.0f), Tooltip("Intensity at which elements are blurred from the center of the screen.")]
    float _blurAmount = 0.2f;
    /// <summary>
    /// Intensity at which elements are blurred from the center of the screen.
    /// </summary>
    public float blurAmount {
        get { return _blurAmount; }
        set { _blurAmount = value; }
    }

    [SerializeField, Range(-1.0f, 1.0f)]
    float x, y;

    /// <summary>
    /// Position of the center of the blur from the center of the screen
    /// </summary>
    public Vector2 offset {
        get { return new Vector2(x, y); }
        set { x = value.x; y = value.y; }
    }

    #endregion

    #region Private Properties

    [SerializeField]
    Shader _shader;
    Material _material;

    #endregion

    #region MonoBehaviour Functions
    
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
        _material.SetFloat("_BlurAmount", _blurAmount);
        _material.SetVector("_Offset", offset);

        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
