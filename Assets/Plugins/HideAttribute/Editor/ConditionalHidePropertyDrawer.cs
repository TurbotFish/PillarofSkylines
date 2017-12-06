using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
		bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

		bool wasEnabled = GUI.enabled;
		GUI.enabled = enabled;
		if (!condHAtt.HideInInspector || enabled) {
			EditorGUI.PropertyField(position, property, label, true);
		}

		GUI.enabled = wasEnabled;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
		bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

		if (!condHAtt.HideInInspector || enabled) {
			return EditorGUI.GetPropertyHeight(property, label);
		} else {
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}

	private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property) {
		bool enabled = true;

		string propertyPath = property.propertyPath;
		string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
		SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

		if (sourcePropertyValue != null) {
            if (condHAtt.isInt) 
                enabled = sourcePropertyValue.intValue == condHAtt.targetValue;
            else
    			enabled = sourcePropertyValue.boolValue;

		} else {
			Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
		}

		return enabled;
	}
}