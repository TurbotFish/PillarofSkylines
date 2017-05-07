using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/CubicDistortion Grr")]
public class CubicDistortion : MonoBehaviour {
    #region Public Properties
    
    [SerializeField, Range(-0.15f, 0.15f), Tooltip("Intensity of the Distortion effect.")]
    float _intensity = -0.1f;
    /// <summary>
    /// Intensity of the Distortion effect.
    /// </summary>
    public float intensity {
        get { return _intensity; }
        set { _intensity = value; }
    }
    #endregion

    #region Private Properties

    [SerializeField] Shader _shader;
    Material _material;
    
    #endregion

    #region MonoBehaviour Functions

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
        _material.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}