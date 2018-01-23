using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingBuilder))]
public class BuildingBuilderEditor : Editor {

	private BuildingBuilder bb;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		bb = target as BuildingBuilder;

		if (GUILayout.Button("Create Building")) {
			Undo.RecordObject(bb, "Create Building");
			bb.CreateBuilding ();
			EditorUtility.SetDirty(bb);
		}
		if (GUILayout.Button("Clear Building")) {
			Undo.RecordObject(bb, "Clear Building");
			bb.ClearBuilding ();
			EditorUtility.SetDirty(bb);
		}
		if (GUILayout.Button("Random Building")) {
			Undo.RecordObject(bb, "Random Building");
			bb.RandomBuilding ();
			EditorUtility.SetDirty(bb);
		}
	}
}
