using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrassScaleVariationTexHolder", fileName = "GrassScaleVariation")]
public class SurfaceGrassScaleVariationMaps : ScriptableObject {

	public Texture2D EastMap;
	public Texture2D WestMap;
	public int mapResolution = 128;
}
