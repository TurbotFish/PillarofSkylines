using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform))]
public class WorldSpaceTransform : DecoratorEditor {

	public WorldSpaceTransform() : base("TransformInspector") { }

	bool worldSpace;

	[SerializeField] Vector3 worldPosition, worldRotation, lossyScale;

	public override void OnInspectorGUI() {

		if (!Selection.activeGameObject)
			return;

		Transform target = Selection.activeGameObject.transform;

		if (worldSpace) {
			worldPosition = target.position;
			worldRotation = target.eulerAngles;
			lossyScale = target.lossyScale;


			EditorGUI.BeginChangeCheck ();
			worldPosition = EditorGUILayout.Vector3Field("World Position", worldPosition);
			if (EditorGUI.EndChangeCheck ()) {
				target.position = worldPosition;
			}

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Vector3Field("World Rotation", worldRotation);
			EditorGUILayout.Vector3Field("Lossy Scale", lossyScale);
			
			EditorGUI.EndDisabledGroup();

		} else {
			base.OnInspectorGUI();
		}
		
		worldSpace = GUILayout.Toggle(worldSpace, " See In World Space");

		Repaint();
	}

}
