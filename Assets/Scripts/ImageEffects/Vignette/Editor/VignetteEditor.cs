using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Vignette))]
public class VignetteEditor : Editor {
    SerializedProperty _falloff, _color, _strength, _scale, _adapt, _power;

    void OnEnable()
    {
        _falloff = serializedObject.FindProperty("_falloff");
        _color = serializedObject.FindProperty("_color");
        _scale = serializedObject.FindProperty("_scale");
        _adapt = serializedObject.FindProperty("adaptToScreen");
        _power = serializedObject.FindProperty("_power");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_falloff);
        EditorGUILayout.PropertyField(_color);
        EditorGUILayout.PropertyField(_power);
        EditorGUILayout.PropertyField(_adapt);

        if (!_adapt.boolValue) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_scale);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}