using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Bool3))]
public class Bool3Drawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        label = EditorGUI.BeginProperty(position, label, property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        contentPosition.width *= 0.2f;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 12f;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("x"), new GUIContent("X"));

        contentPosition.x += contentPosition.width + 1f;
        EditorGUIUtility.labelWidth = 12f;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("y"), new GUIContent("Y"));

        contentPosition.x += contentPosition.width + 1f;
        EditorGUIUtility.labelWidth = 12f;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("z"), new GUIContent("Z"));

        EditorGUI.EndProperty();
    }
}
