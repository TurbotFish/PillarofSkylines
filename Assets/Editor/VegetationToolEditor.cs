using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VegetationTool))]
public class VegetationToolEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		VegetationTool tool = (VegetationTool)target;
		if (GUILayout.Button("Instantiate Vegetation"))
		{
			tool.InstantiateVegetation();
		}

		if (GUILayout.Button("Delete All Groups"))
		{
			List<GameObject> children = new List<GameObject>();
			foreach(Transform child in tool.transform) children.Add(child.gameObject);
			children.ForEach(child => DestroyImmediate(child));
		}
	}
}
