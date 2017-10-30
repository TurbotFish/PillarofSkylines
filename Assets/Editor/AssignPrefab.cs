using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class AssignPrefab : EditorWindow {

	[SerializeField] GameObject prefab;

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
		bool flag = false;
		for (int i = 0; i < components.Length; i++) {
			if (ComponentUtility.CopyComponent(components[i])) {
				var comp = newTarget.GetComponent(components[i].GetType());
				flag = comp ? ComponentUtility.PasteComponentValues(comp) : ComponentUtility.PasteComponentAsNew(newTarget);
			}
		}

		ObjectReferenceUtility.ReplaceAllReferences(target, newTarget);

		Undo.DestroyObjectImmediate(target);

		Selection.activeGameObject = newTarget;
	}

}
