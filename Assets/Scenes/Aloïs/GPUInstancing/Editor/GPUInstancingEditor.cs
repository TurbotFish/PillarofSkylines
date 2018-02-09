using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(GPUInstancingBehaviour))]
public class GPUInstancingEditor : Editor {

	public string[] layers;
	SerializedProperty behaviourLayer;
	public int layerIndex;

	public override void OnInspectorGUI (){
		serializedObject.Update ();
		base.OnInspectorGUI ();

		//set layer
		layerIndex = EditorGUILayout.Popup ("Layer", layerIndex, layers);
		behaviourLayer.intValue = layerIndex;


		serializedObject.ApplyModifiedProperties ();

	}



	void OnEnable(){
		List<string> layerList = new List<string> ();
		behaviourLayer = serializedObject.FindProperty ("layer");
		layerIndex = behaviourLayer.intValue;

		for (int i = 0; i < 30; i++) {
			string currentLayer = LayerMask.LayerToName (i);

			if (currentLayer != "")
				layerList.Add (currentLayer);
			
		}

		layers = layerList.ToArray ();


	}


}
