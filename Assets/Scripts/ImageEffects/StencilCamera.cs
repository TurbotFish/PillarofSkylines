using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class StencilCamera : MonoBehaviour {

    #region Private Properties

    [SerializeField] Shader _shader;
    Material _material;

    #endregion

    #region MonoBehaviour Functions

    void OnValidate() {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
    }

    [HideInInspector]
    public Vector2 camSpeed;
    Vector2 lastCamSpeed;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_material == null) {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
        
        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
