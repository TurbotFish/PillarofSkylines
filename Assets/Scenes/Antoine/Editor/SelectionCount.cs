using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SelectionCount : MonoBehaviour {



	void OnSceneGUI () {
		GUI.Label(new Rect(10, 10, 100, 20), Selection.objects.Length.ToString());
	}
}
