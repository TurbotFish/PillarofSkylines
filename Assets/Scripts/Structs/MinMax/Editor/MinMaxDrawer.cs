using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        label = EditorGUI.BeginProperty(position, label, property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        contentPosition.width *= 0.5f;
        contentPosition.width -= 2f;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 26f;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("min"), new GUIContent("Min"));

        contentPosition.x += contentPosition.width + 4f;
        EditorGUIUtility.labelWidth = 28f;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("max"), new GUIContent("Max"));
        
        EditorGUI.EndProperty();
    }
}
