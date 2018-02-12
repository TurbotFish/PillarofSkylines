using UnityEditor;
using UnityEngine;

public class GPUIMatrixBaker : EditorWindow {

	[SerializeField] GPUInstancingBehaviour sObj;
	[SerializeField] GameObject obj;

	[MenuItem("Tools/Bake GPUI Matrices", false, 90)]
	public static void DisplayWindow(){
		GetWindow<GPUIMatrixBaker> ("Matrix Baker");
	}

	void OnGUI(){
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("testtestonetwoonetwo", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		sObj = EditorGUILayout.ObjectField ("GPUI Behaviour", sObj, typeof(GPUInstancingBehaviour), false) as GPUInstancingBehaviour;
		EditorGUILayout.Space ();

		obj = EditorGUILayout.ObjectField ("Seeder Object", obj, typeof(GameObject), true) as GameObject;
		EditorGUILayout.Space ();
		if (obj != null && obj.GetComponent<MeshFilter> ().sharedMesh.vertexCount > 1023) {
			EditorGUILayout.HelpBox ("Mesh has more than 1024 vertices, I need to figure out what to do.", MessageType.Error);

		}

		EditorGUI.BeginDisabledGroup (sObj == null || obj == null);
		if (GUILayout.Button ("Bake Matrices")) {
			BakeMatrices (sObj, obj);
		}

		EditorGUI.EndDisabledGroup ();

//		if (GUILayout.Button ("Debug Matrices")) {
//			Debug.Log (sObj.matrices [0]);
//		}
	}


	void BakeMatrices(GPUInstancingBehaviour _sObj, GameObject _obj){
		Debug.Log ("I DO THE BAKING");

		Matrix4x4[] transforms = new Matrix4x4[sObj.instances];
		Matrix4x4 objObject2World = obj.transform.localToWorldMatrix;
		Quaternion objRotation = obj.transform.rotation;
		Vector3 objScale = Vector3.one;

		Mesh objMesh = obj.GetComponent<MeshFilter> ().sharedMesh;

		for (int i = 0; i < objMesh.vertexCount; i++) {
			transforms [i] = Matrix4x4.TRS (objObject2World.MultiplyPoint (objMesh.vertices [i]), objRotation, objScale);
		}

		sObj.matrices = transforms;
	}

}
