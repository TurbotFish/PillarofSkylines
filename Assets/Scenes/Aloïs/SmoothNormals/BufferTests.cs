using UnityEngine;

[ExecuteInEditMode]
public class BufferTests : MonoBehaviour {

	public Material mat;

	void Start(){
		Camera cam = Camera.main;
		cam.depthTextureMode = DepthTextureMode.DepthNormals;

	}

//	void OnRenderImage(RenderTexture src, RenderTexture dst){
//		Graphics.Blit (src, dst, mat);
//	}
}
