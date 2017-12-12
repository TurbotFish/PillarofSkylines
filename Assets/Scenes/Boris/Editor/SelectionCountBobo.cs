using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class SelectionCountBobo : MonoBehaviour {

	public float count;

	// Update is called once per frame
	void Update () {
		count = Selection.objects.Length;
		Debug.Log (Selection.objects.Length);
	}
}
