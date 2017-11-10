using UnityEngine;
using UnityEditor;

public class AnimationCurveGUI
{
    private static Rect SubtractPopupWidth(Rect position)
    {
        position.width -= 18f;
        return position;
    }

    private static Rect GetPopupRect(Rect position)
    {
        position.xMin = position.xMax - 13f;
        return position;
    }

    #region EditorGUILayout
    public static AnimationCurve CurveField(AnimationCurve value, params GUILayoutOption[] options)
    {
        Rect position = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField, options);
        return CurveField(position, value);
    }

    public static AnimationCurve CurveField(string label, AnimationCurve value, params GUILayoutOption[] options)
    {
        Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
        return CurveField(position, label, value);
    }

    public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, params GUILayoutOption[] options)
    {
        Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
        return CurveField(position, label, value);
    }
    #endregion

    #region EditorGUI
    public static AnimationCurve CurveField(Rect position, AnimationCurve value)
    {
        AnimationCurve animationCurve = EditorGUI.CurveField(SubtractPopupWidth(position), value);
        AnimationCurvePopupMenu.Show(GetPopupRect(position), animationCurve, null);
        return animationCurve;
    }

    public static AnimationCurve CurveField(Rect position, string label, AnimationCurve value)
    {
        return CurveField(position, new GUIContent(label), value);
    }

    public static AnimationCurve CurveField(Rect position, GUIContent label, AnimationCurve value)
    {
        AnimationCurve animationCurve = EditorGUI.CurveField(SubtractPopupWidth(position), label, value);
        AnimationCurvePopupMenu.Show(GetPopupRect(position), animationCurve, null);
        return animationCurve;
    }
    #endregion

    public static void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.CurveField(SubtractPopupWidth(position), property, Color.green, default(Rect));
        AnimationCurvePopupMenu.Show(GetPopupRect(position), null, property);
        EditorGUI.EndProperty();
    }
}
