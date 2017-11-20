using UnityEngine;

[ExecuteInEditMode]
public class DebugReplacementShading : MonoBehaviour {

	public DebugMode debugMode;

	[Header("Shaders")]
	public Shader checkerShader;
	public Shader slopeCheckShader;

	[Header("Checker Parameters")]
	[Range(0.01f,100)]
	public float checkerScale = 1f;
	[Range(0,1)]
	public float contrastMin = .2f;
	[Range(0,1)]
	public float contrastMax = .5f;

	bool replacementReady;

	public enum DebugMode {
        None,
		SlopeCheck,
		Checker
	};

	void OnValidate(){
		if (debugMode == DebugMode.Checker) {
			debugMode = DebugMode.Checker;
		} 
		if (debugMode == DebugMode.SlopeCheck) {
			debugMode = DebugMode.SlopeCheck;
		}

		if (debugMode != DebugMode.Checker) {
		
			if (contrastMax < contrastMin) {
				Debug.LogError ("contrastMax has to be greater than contrastMin, you lovely idiot");
				contrastMax = contrastMin;
			}
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
		if(!replacementReady || debugMode == DebugMode.None)
			return;

		if (debugMode == DebugMode.Checker) {
			Shader.SetGlobalFloat("_CheckerScale", .5f / checkerScale);
			Shader.SetGlobalFloat("_CheckerMinContrast", contrastMin);
			Shader.SetGlobalFloat("_CheckerMaxContrast", contrastMax);
		}

		Shader repShader = null;
		if (debugMode == DebugMode.Checker) {
			repShader = checkerShader;
		} else if (debugMode == DebugMode.SlopeCheck) {
			repShader = slopeCheckShader;
		}

		GetComponent<Camera>().SetReplacementShader(repShader,"RenderType");
	}
}
