using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GPUISeeder)), CanEditMultipleObjects]
public class GPUISeederEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		GPUISeeder _seeder = (GPUISeeder)target;

		if (GUILayout.Button ("Bake Matrices")) {
			_seeder.BakeGPUIData ();
		}

	}
}
