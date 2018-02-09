using UnityEditor;
using UnityEngine;

public class GPUIMatrixBaker : EditorWindow {

	[SerializeField] ScriptableObject sObj;
	[SerializeField] GameObject obj;

	[MenuItem("Tools/Bake GPUI Matrices", false, 90)]
	public static void DisplayWindow(){
		GetWindow<GPUIMatrixBaker> ("Matrix Baker");
	}

	void OnGUI(){
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("testtestonetwoonetwo", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		sObj = EditorGUILayout.ObjectField ("GPUI Behaviour", sObj, typeof(ScriptableObject), false) as ScriptableObject;
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
	}


	void BakeMatrices(ScriptableObject _sObj, GameObject _obj){
		Debug.Log ("DIDIDI");
	}

}
