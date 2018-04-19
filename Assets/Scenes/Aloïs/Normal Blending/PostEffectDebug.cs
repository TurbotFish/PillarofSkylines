using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectDebug : MonoBehaviour {

	public Shader shader;
	Material mat;

	[Range(0,32)]
	public int iterations;

	void OnRenderImage(RenderTexture source, RenderTexture destination){
		//if (mat == null) {
			mat = new Material (shader);
			mat.hideFlags = HideFlags.HideAndDontSave;
		//}

		RenderTexture rt1 = RenderTexture.GetTemporary (source.width, source.height);
		RenderTexture rt2 = RenderTexture.GetTemporary (source.width, source.height);

		Graphics.Blit (source, rt1, mat, 0);
		Graphics.Blit (rt1, rt2, mat, 1);

		for (int i = 0; i < iterations; i++) {
			Graphics.Blit (rt2, rt1, mat, 0);
			Graphics.Blit (rt1, rt2, mat, 1);
		}

		Graphics.Blit (rt2, destination);
	}
}
