using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioLibrary.AudioAsset))]
public class AudioAssetDrawer : PropertyDrawer
{

    private static Color pro = new Color(0.7f, 0.7f, 0.7f, 1f);
    private static Color free = new Color(0, 0, 0, 1);

    GUIStyle style = new GUIStyle();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        style.wordWrap = true;
        style.normal.textColor = EditorGUIUtility.isProSkin ? pro : free;
        
        label = EditorGUI.BeginProperty(position, label, property);

        float width = position.width;

        position.width *= 0.4f;


        property.FindPropertyRelative("name").stringValue = EditorGUI.TextField(position, property.FindPropertyRelative("name").stringValue, style);

        //EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), new GUIContent(), style);

        position.x += position.width;

        position.width = width * 0.6f;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("clip"), new GUIContent());
        
        EditorGUI.EndProperty();
    }
}
