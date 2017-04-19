using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CubicDistortion))]
public class CubicDistortionEditor : Editor {
    SerializedProperty _intensity;

    void OnEnable() {
        _intensity = serializedObject.FindProperty("_intensity");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_intensity);

        serializedObject.ApplyModifiedProperties();
    }
}