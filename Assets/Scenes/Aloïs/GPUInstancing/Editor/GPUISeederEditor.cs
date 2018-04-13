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

	Color scaleButtonsColor1 = new Color (.2f, .2f, .2f, .4f);

	int bakingState = 1;

	SerializedProperty _col;
	bool colorsPainted;
	SerializedProperty _colorsPainted;

	Vector3 scaleMult = Vector3.one;
	bool scaleUnchanged;

	public override void OnInspectorGUI(){

		base.OnInspectorGUI ();

		colorsPainted = serializedObject.FindProperty ("_colorsWerePainted").boolValue;

		foreach (GPUISeeder item in targets) {
			if (Time.frameCount % 10 == 0) {
				//0 : baked
				//1 : rebake needed
				//2 : never baked
				bakingState = item.AreMatricesUpToDate ();


				scaleUnchanged = item.IsScaleUpToDate ();
				//if doesn't need rebake but scale has changed, need rebake
				bakingState = bakingState == 0 && !scaleUnchanged ? 1 : bakingState;

				colorsPainted = bakingState != 0 ? false : colorsPainted;

				ColorizeButton (bakingState);
			}
		}

		///Scale multiplier stuff
		//set inspector values to seeder values
		foreach (GPUISeeder item in targets) {
			scaleMult = item.scaleMultiplier;
		}
		scaleMult = EditorGUILayout.Vector3Field ("Scale Multiplier", scaleMult);
		//set seeder values to inspector values
		GUI.backgroundColor = scaleButtonsColor1;
		if (!scaleUnchanged) {

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Paint map")) {

				foreach (GPUISeeder item in targets) {
					item.scaleMultiplier = scaleMult;
					item.PaintScaleVariationMap ();
				}
			}


			if (GUILayout.Button ("Set to map value")) {
				foreach (GPUISeeder item in targets) {
					item.SetToMapValue ();
					scaleMult = item.scaleMultiplier;
				}
			}

			EditorGUILayout.EndHorizontal ();
		}

		foreach (GPUISeeder item in targets) {
			item.scaleMultiplier = scaleMult;
		}
		///

		if (!scaleUnchanged) {
			EditorGUILayout.HelpBox ("Sort out scale multiplier before baking.", MessageType.Info);
		}

		EditorGUI.BeginDisabledGroup (!scaleUnchanged);
		///Matrix baking
		GUI.backgroundColor = buttonColor;
		if (GUILayout.Button ("Bake Matrices")) {
			foreach (GPUISeeder item in targets) {
				item.BakeGPUIData ();
				ColorizeButton ();
			}
		}
		///
		EditorGUI.EndDisabledGroup();



		EditorGUILayout.Space ();


		///Grass colour
		EditorGUI.BeginDisabledGroup (bakingState != 0);

		GUI.backgroundColor = Color.white;

		paintMode = (PaintMode)EditorGUILayout.EnumPopup ("Paint Mode", paintMode);

		if (paintMode == PaintMode.Picker) {

			foreach (GPUISeeder item in targets) {
				paintColor = item._paintColor;
			}
			EditorGUI.BeginChangeCheck ();
			paintColor = EditorGUILayout.ColorField ("Grass Colour", paintColor);
			if (EditorGUI.EndChangeCheck ()) {
				foreach (GPUISeeder item in targets) {
					item._paintColor = paintColor;
				}
			}	

		}

		GUI.backgroundColor = colorsPainted ? colorBaked : colorNeverBaked;
		if (GUILayout.Button ("Paint Grass Colour Map")) {
			foreach (GPUISeeder item in targets) {
				item.PaintSurfaceTexture (paintMode == PaintMode.Vertex, paintColor);

			}
			colorsPainted = true;
		}
		EditorGUI.EndDisabledGroup ();

		GUI.backgroundColor = Color.white;
		if (bakingState != 0) {
			EditorGUILayout.HelpBox ("Matrices must be baked to paint.", MessageType.Info);
		}
		serializedObject.FindProperty ("_colorsWerePainted").boolValue = colorsPainted;
		serializedObject.ApplyModifiedProperties ();
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
