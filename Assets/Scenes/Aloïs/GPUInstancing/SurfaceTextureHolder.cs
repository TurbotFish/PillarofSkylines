using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrassColorTexHolder", fileName = "TextureHolder")]
public class SurfaceTextureHolder : ScriptableObject {

	public Texture2D eastTex;
	public Texture2D westTex;
	public int mapResolution = 64;
}
