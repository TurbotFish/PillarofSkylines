using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(GradientFog))]
[AddComponentMenu("Image Effects/GradientFog (Grr) - Secondary Gradient")]
public class SecondaryGradient : MonoBehaviour {

	[Range(0, 1)]
	public float interpolation = 0;
	public Gradient gradient;
	[Header("Distance")]
	public float start = 0;
	public float end = 500;

	[Header("Primary Fog")]
	public float _start = 0;
	public float _end = 500;

	GradientFog fog;

	void Start() {
		if (!fog)
			fog = GetComponent<GradientFog>();
		_start = fog.startDistance;
		_end = fog.endDistance;
	}

	public void Lerp(float t) {
		interpolation = t;
		OnValidate();
	}

	void OnValidate() {
		if (!fog)
			fog = GetComponent<GradientFog>();
		
		fog.secondaryGradient = gradient;
		fog.gradientLerp = interpolation;
		fog.GenerateTexture();
		fog.startDistance = Mathf.Lerp(_start, start, interpolation);
		fog.endDistance = Mathf.Lerp(_end, end, interpolation);
	}
}
