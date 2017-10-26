
using UnityEngine;

[ExecuteInEditMode]
public class CheckerReplacementShading : MonoBehaviour {

	public Shader checkerShaderWorld;
	[Range(0.01f,100)]
	public float checkerScale = 1f;

	[Range(0,1)]
	public float contrastMin = .2f;

	[Range(0,1)]
	public float contrastMax = .5f;

	bool replacementReady;

	void OnValidate(){
		if (contrastMax < contrastMin) {
			Debug.LogError ("contrastMax has to be greater than contrastMin, you lovely idiot");
			contrastMax = contrastMin;
		}
		DoTheShaderThing();
	}

	void OnEnable(){
		replacementReady = true;
		DoTheShaderThing();
	}

	void OnDisable(){
		replacementReady = false;
		GetComponent<Camera>().ResetReplacementShader();
	}

	void DoTheShaderThing(){
		if(!replacementReady)
			return;
		Shader.SetGlobalFloat("_CheckerScale", .5f / checkerScale);
		Shader.SetGlobalFloat("_CheckerMinContrast", contrastMin);
		Shader.SetGlobalFloat("_CheckerMaxContrast", contrastMax);
		GetComponent<Camera>().SetReplacementShader(checkerShaderWorld,"RenderType");
	}
}
