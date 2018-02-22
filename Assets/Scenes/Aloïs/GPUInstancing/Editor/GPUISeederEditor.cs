using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GPUISeeder))]
public class GPUISeederEditor : Editor {

	Color buttonColor;

	public enum PaintMode {Vertex, Picker};
	public PaintMode paintMode = PaintMode.Picker;
	public Color paintColor = new Color (.1f, .72f, .03f);

	Color colorBaked = new Color (.3f, .65f, 1f, .5f);
	Color colorRebake = new Color (1f, .6f, .2f, .5f);
	Color colorNeverBaked = new Color (1f, .31f, .31f, .5f);

	int bakingState = 1;
	public bool colorsAreBaked;

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		//int _bakingState = 5;

		foreach (GPUISeeder item in targets) {
			if (Time.frameCount % 60 == 0) {
				//0 : baked
				//1 : rebake needed
				//2 : never baked
				bakingState = item.AreMatricesUpToDate ();
				colorsAreBaked = bakingState != 0 ? false : colorsAreBaked;
				ColorizeButton (bakingState);
			}
		}


		GUI.backgroundColor = buttonColor;
		if (GUILayout.Button ("Bake Matrices")) {
			foreach (GPUISeeder item in targets) {
				item.BakeGPUIData ();
				ColorizeButton ();
			}
		}

		EditorGUILayout.Space ();

		EditorGUI.BeginDisabledGroup (bakingState != 0);

		GUI.backgroundColor = Color.white;
		paintMode = (PaintMode)EditorGUILayout.EnumPopup ("Paint Mode", paintMode);
		if (paintMode == PaintMode.Picker) {
			paintColor = EditorGUILayout.ColorField ("Grass Colour", paintColor);

		}

		GUI.backgroundColor = colorsAreBaked ? colorBaked : colorNeverBaked;
		if (GUILayout.Button ("Paint Grass Colour Map")) {
			foreach (GPUISeeder item in targets) {
				item.PaintSurfaceTexture ();

			}
			colorsAreBaked = true;
		}
		EditorGUI.EndDisabledGroup ();

		GUI.backgroundColor = Color.white;
		if (bakingState != 0) {
			EditorGUILayout.HelpBox ("Matrices must be baked to paint.", MessageType.Info);
		}

	}

	void OnEnable(){
		ColorizeButton ();
	}

	void ColorizeButton(){
		foreach (GPUISeeder item in targets) {

			int _bakingState = item.AreMatricesUpToDate();
			if (_bakingState == 0) {
				buttonColor = colorBaked;
			} else if (_bakingState == 1) {
				buttonColor = colorRebake;
			} else if (_bakingState == 2) {
				buttonColor = colorNeverBaked;
			}
		}
		//GPUISeeder _seeder = (GPUISeeder)target;

	
	}

	void ColorizeButton(int _state){
		foreach (GPUISeeder item in targets) {
			if (_state == 0) {
				buttonColor = colorBaked;
			} else if (_state == 1) {
				buttonColor = colorRebake;
			} else if (_state == 2) {
				buttonColor = colorNeverBaked;
			}
		}
	}
}
