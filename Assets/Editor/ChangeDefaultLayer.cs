using UnityEngine;
using UnityEditor;


public class ChangeDefaultLayer : EditorWindow {


	[MenuItem("GameObject/Change Default Layer", false, 10)]
	public static void ShowWindow() {
		GetWindow(typeof(ChangeDefaultLayer));
		GetWindow<ChangeDefaultLayer>().Show();
	}

	public int selectedLayer;

	void OnGUI() {
		Event e = Event.current;

		EditorGUILayout.LabelField("Change the layer of every child currently on the Default layer", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		selectedLayer = EditorGUILayout.LayerField ("New Layer: ", selectedLayer);

		if (GUILayout.Button ("Apply") || e.isKey && e.keyCode == KeyCode.Return) {

			foreach (GameObject target in Selection.gameObjects)
				ApplyNewLayer (target, selectedLayer);

			Close ();
		}
		Repaint ();
	}

	void ApplyNewLayer(GameObject target, int selectedLayer)
    {
        if (target.gameObject.layer == 0)
            target.layer = selectedLayer;
		foreach (Transform child in target.transform) {
			ApplyNewLayer(child.gameObject, selectedLayer);
		}
	}
}
