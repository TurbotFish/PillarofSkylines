using UnityEngine;

public class WindManager : MonoBehaviour {

	public Texture2D windTex;
	[Range(0,10)]
	public int scrollSpeed;

	float windForce;
	int xForce;
	int yForce;
	int texRes;

	void Start(){
		Shader.SetGlobalTexture ("_WindTex", windTex);
	}

	void Update(){
		
		//Shader.SetGlobalFloat ("_WindIntensity", windForce);
	}
}
