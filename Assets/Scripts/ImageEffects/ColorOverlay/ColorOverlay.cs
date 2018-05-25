using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/ColorOverlay (Grr)")]
public class ColorOverlay : MonoBehaviour {
    #region Public Properties
    
    [SerializeField, Range(0.0f, 1.0f), Tooltip("Intensity of the Overlay.")]
	float _intensity = 0.5f;
    /// <summary>
    /// Closeness of the Vignette to the center of the screen.
    /// </summary>
    public float intensity {
        get { return _intensity; }
        set { _intensity = value; }
    }

    [SerializeField, ColorUsage(false), Tooltip("The Color of the Overlay.")]
    Color _color = Color.black;
    /// <summary>
    /// The Color of the Vignetting effect.
    /// </summary>
    public Color color {
        get { return _color; }
        set { _color = value; }
    }

	[System.Serializable]
	public enum BlendMode { Normal, Multiply, Screen, Overlay }

	[SerializeField, Tooltip("How the color blends with the Screen.")]
	BlendMode _blend = BlendMode.Normal;
	/// <summary>
	/// How the color blends with the Screen.
	/// </summary>
	public BlendMode blend {
		get { return _blend; }
		set {
			_blend = value;
			OnValidate();
		}
	}

    #endregion

    #region Private Properties

    [SerializeField] Shader _shader;
    Material _material;

    [SerializeField]
    bool adaptToScreen;

	#endregion

	#region MonoBehaviour Functions

	void OnValidate() {
		if (_material == null) {
			_material = new Material(_shader);
			_material.hideFlags = HideFlags.DontSave;
		}

		switch (_blend) {
			default:
			case BlendMode.Normal:
				_material.DisableKeyword("_MULTIPLY");
				_material.DisableKeyword("_SCREEN");
				_material.DisableKeyword("_OVERLAY");
				break;
			case BlendMode.Multiply:
				_material.EnableKeyword("_MULTIPLY");
				_material.DisableKeyword("_SCREEN");
				_material.DisableKeyword("_OVERLAY");
				break;
			case BlendMode.Screen:
				_material.DisableKeyword("_MULTIPLY");
				_material.EnableKeyword("_SCREEN");
				_material.DisableKeyword("_OVERLAY");
				break;
			case BlendMode.Overlay:
				_material.DisableKeyword("_MULTIPLY");
				_material.DisableKeyword("_SCREEN");
				_material.EnableKeyword("_OVERLAY");
				break;
		}
	}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        _material.SetFloat("_Intensity", _intensity);
        _material.SetColor("_Color", _color);

        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
