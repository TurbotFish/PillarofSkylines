using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GradientFog))]
public class GradientFogEditor : Editor {
    SerializedProperty _fogStart, _fogEnd, _gradient, _drawOverSkybox;

    void OnEnable() {
        _fogStart = serializedObject.FindProperty("_start");
        _fogEnd = serializedObject.FindProperty("_end");
        _gradient = serializedObject.FindProperty("gradient");
        _drawOverSkybox = serializedObject.FindProperty("_drawOverSkybox");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_gradient);

        EditorGUILayout.LabelField("Distance", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_fogStart);
        EditorGUILayout.PropertyField(_fogEnd);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.PropertyField(_drawOverSkybox);

        serializedObject.ApplyModifiedProperties();
    }
}