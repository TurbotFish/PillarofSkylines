using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class AssignPrefab : EditorWindow {

	[SerializeField] GameObject prefab;
	[SerializeField] bool keepChildren;

	[MenuItem("Tools/Assign Prefab", false, 90)]
	public static void ShowWindow() {
		GetWindow(typeof(AssignPrefab));
		GetWindow<AssignPrefab>().Show();
	}

	private void OnGUI() {
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Assign Prefab to selected GameObject(s):", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		prefab = EditorGUILayout.ObjectField("Prefab to Assign", prefab, typeof(GameObject), false) as GameObject;
		EditorGUILayout.Space();

		foreach (GameObject target in Selection.gameObjects) {
			if (target.transform.childCount > 0) {
				if (!keepChildren)
					EditorGUILayout.HelpBox(target.name + " has children which will be deleted upon action.", MessageType.Error);

				keepChildren = EditorGUILayout.Toggle("Keep Children ", keepChildren);

				if (keepChildren)
					EditorGUILayout.HelpBox("Keeping children might lead to duplicates.", MessageType.Warning);

				EditorGUILayout.Space();
				break;
			}
		}
		
		EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);
		if (GUILayout.Button("Assign Prefab")) {

			foreach(GameObject target in Selection.gameObjects) 
				DoPrefabAction(target, prefab);
		}
		if (GUILayout.Button("Remove Prefab", EditorStyles.miniButton)) {

			foreach (GameObject target in Selection.gameObjects)
				DoPrefabAction(target, false);
		}
		EditorGUI.EndDisabledGroup();

		Repaint();
	}

	void DoPrefabAction(GameObject target, bool connect) {
		if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
			return;

		GameObject newTarget;
		if(connect)
			Undo.RegisterCreatedObjectUndo(newTarget = (GameObject)PrefabUtility.InstantiatePrefab(prefab), "Created Prefab Instance of " + (prefab ? prefab.name : "null"));
		else
			Undo.RegisterCreatedObjectUndo(newTarget = new GameObject(), "Created non-Prefab Instance of " + (prefab ? prefab.name : "null"));

		Component[] components = target.GetComponents<Component>();

		newTarget.name = target.name;
		newTarget.transform.parent = target.transform.parent;
		for (int i = 0; i < components.Length; i++) {
			if (ComponentUtility.CopyComponent(components[i])) {
				var comp = newTarget.GetComponent(components[i].GetType());
				if (comp)
					ComponentUtility.PasteComponentValues(comp);
				else
					ComponentUtility.PasteComponentAsNew(newTarget);
			}
		}

		ObjectReferenceUtility.ReplaceAllReferences(target, newTarget);

		if (keepChildren) {
			foreach (Transform child in target.transform) {
				Vector3 localPos = child.localPosition;
				Quaternion localRot = child.localRotation;
				Vector3 localScale = child.localScale;
				child.parent = newTarget.transform;
				child.localPosition = localPos;
				child.localRotation = localRot;
				child.localScale = localScale;
			}
		}
		
		Undo.DestroyObjectImmediate(target);

		Selection.activeGameObject = newTarget;
	}

}
