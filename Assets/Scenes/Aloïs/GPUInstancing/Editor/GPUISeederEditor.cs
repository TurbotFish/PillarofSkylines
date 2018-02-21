using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GPUISeeder))]
public class GPUISeederEditor : Editor {

	Color buttonColor;


	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		GUI.backgroundColor = buttonColor;
		if (GUILayout.Button ("Bake Matrices")) {
			foreach (GPUISeeder item in targets) {
				item.BakeGPUIData ();
				ColorizeButton ();
			}
		}

		GUI.backgroundColor = Color.white;
		if (GUILayout.Button ("Paint Grass Colour Map")) {
			foreach (GPUISeeder item in targets) {
				item.PaintSurfaceTexture ();
			}
		}

	}

	void OnEnable(){
		ColorizeButton ();
	}

	void ColorizeButton(){
		foreach (GPUISeeder item in targets) {
			buttonColor = item.AreMatricesUpToDate();
			//Debug.Log ("GAG");
		}
		//GPUISeeder _seeder = (GPUISeeder)target;

	
	}
}
