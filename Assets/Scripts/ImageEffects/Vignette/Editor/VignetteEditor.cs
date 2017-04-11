using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Vignette))]
public class VignetteEditor : Editor
{
    SerializedProperty _falloff, _color, _strength;

    void OnEnable()
    {
        _falloff = serializedObject.FindProperty("_falloff");
        _color = serializedObject.FindProperty("_color");
        _strength = serializedObject.FindProperty("_strength");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_falloff);
        EditorGUILayout.PropertyField(_color);
        EditorGUILayout.PropertyField(_strength);

        serializedObject.ApplyModifiedProperties();
    }
}