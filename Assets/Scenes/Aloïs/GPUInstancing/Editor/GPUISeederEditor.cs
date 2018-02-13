using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GPUISeeder))]
public class GPUISeederEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		//GPUISeeder _seeder = (GPUISeeder)target;

		if (GUILayout.Button ("Bake Matrices")) {
			foreach (GPUISeeder item in targets) {
				item.BakeGPUIData ();
			}
			//_seeder.BakeGPUIData ();
		}

	}
}
