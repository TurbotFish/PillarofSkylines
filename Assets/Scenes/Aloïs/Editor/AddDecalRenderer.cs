using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AddDecalRenderer : Editor {

	DecalRenderer _DR;

	void OnEnable(){

		if (_DR == null) {
			Debug.Log ("GAGAGA");
			_DR = SceneView.currentDrawingSceneView.camera.gameObject.AddComponent<DecalRenderer> ();
		}
	}
}
