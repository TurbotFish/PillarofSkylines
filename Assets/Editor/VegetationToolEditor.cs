using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VegetationTool))]
public class VegetationToolEditor : Editor {

	GUIStyle redButton;
	GUIStyle greenButton;
	Texture2D greenTexture;
	Texture2D redTexture;

	public void OnEnable()
	{
		redTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		redTexture.SetPixel(0, 0, Color.red);
		redTexture.Apply();

		greenTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		greenTexture.SetPixel(0, 0, Color.green);
		greenTexture.Apply();
	}

	public override void OnInspectorGUI()
	{
		greenButton = new GUIStyle(GUI.skin.button);
		greenButton.normal.background = greenTexture;
		redButton = new GUIStyle(GUI.skin.button);
		redButton.normal.background = redTexture;


		DrawDefaultInspector();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		VegetationTool tool = (VegetationTool)target;
		if (GUILayout.Button("Instantiate Vegetation", greenButton))
		{
			tool.InstantiateVegetation();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (GUILayout.Button("Delete All Groups", redButton))
		{
			List<GameObject> children = new List<GameObject>();
			foreach(Transform child in tool.transform) children.Add(child.gameObject);
			children.ForEach(child => DestroyImmediate(child));
		}
	}
}
