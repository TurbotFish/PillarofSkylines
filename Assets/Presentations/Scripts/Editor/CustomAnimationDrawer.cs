using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

[CustomPropertyDrawer(typeof(CustomAnimation))]
public class CustomAnimationDrawer : PropertyDrawer
{
	const float rows = 3;


	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		EditorGUI.BeginProperty(pos, label, prop);

		SerializedProperty fade = prop.FindPropertyRelative("fade");
		SerializedProperty direction = prop.FindPropertyRelative("direction");
		SerializedProperty targetValue = prop.FindPropertyRelative("targetValue");
		SerializedProperty duration = prop.FindPropertyRelative("duration");
		SerializedProperty tween = prop.FindPropertyRelative("tween");
		SerializedProperty tmp = prop.FindPropertyRelative("tmp");


		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 2;

		Rect tweenRect = new Rect(pos.x, pos.y+3, pos.width, pos.height/rows);
		Rect tmpRect = new Rect(pos.x, pos.y+3, pos.width, pos.height/rows);

		Rect fadeLabelRect = new Rect(pos.x, pos.y += (pos.height/rows + 6), 45*EditorGUI.indentLevel, pos.height);
		Rect fadeRect = new Rect(pos.x += 60, pos.y, 30*EditorGUI.indentLevel, pos.height);

		Rect directionRect = new Rect(pos.x+35, pos.y, 40*EditorGUI.indentLevel, pos.height/rows);
		Rect targetValueRect = new Rect(pos.x+95, pos.y, 30*EditorGUI.indentLevel, pos.height/rows);
		Rect durationLabelRect = new Rect(pos.x+135, pos.y, 45*EditorGUI.indentLevel, pos.height/rows);
		Rect duractionRect = new Rect(pos.x + 190, pos.y, 30*EditorGUI.indentLevel, pos.height/rows);


		EditorGUI.LabelField(fadeLabelRect, "fade? ->");
		EditorGUI.PropertyField(fadeRect, fade, GUIContent.none);

		if (fade.boolValue)
		{
			EditorGUI.PropertyField(tmpRect, tmp, GUIContent.none);

			EditorGUI.LabelField(fadeLabelRect, "fade? ->");
			EditorGUI.PropertyField(directionRect, direction, GUIContent.none);
			EditorGUI.PropertyField(targetValueRect, targetValue, GUIContent.none);
			EditorGUI.LabelField(durationLabelRect, "duration:");
			EditorGUI.PropertyField(duractionRect, duration, GUIContent.none);
		}
		else
		{
			EditorGUI.PropertyField(tweenRect, tween, GUIContent.none);
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		return base.GetPropertyHeight(prop,label)*(rows);
	}

}
