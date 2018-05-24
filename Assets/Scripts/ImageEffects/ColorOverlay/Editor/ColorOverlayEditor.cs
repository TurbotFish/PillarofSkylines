using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ColorOverlay))]
public class ColorOverlayEditor : Editor {
	SerializedProperty _intensity, _color, _blend;

	void OnEnable()
	{
		_intensity = serializedObject.FindProperty("_intensity");
		_color = serializedObject.FindProperty("_color");
		_blend = serializedObject.FindProperty("_blend");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(_color);
		EditorGUILayout.PropertyField(_intensity);
		EditorGUILayout.PropertyField(_blend);

		serializedObject.ApplyModifiedProperties();
	}
}