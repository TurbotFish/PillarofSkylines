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

		if (GUILayout.Button("Update Building")) {
			Undo.RecordObject(bb, "Update Building");
			bb.UpdateBuilding ();
			EditorUtility.SetDirty(bb);
		}
	}
}
