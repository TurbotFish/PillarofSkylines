using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[CanEditMultipleObjects]
[CustomEditor(typeof(HierarchyStyle))]
public class HierarchyStyleEditor : Editor {
	SerializedProperty textColor, backgroundColor, comment, commentColor;

	static HierarchyStyleEditor() {
		EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItem;
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem;
	}

	void OnEnable() {
		textColor = serializedObject.FindProperty("nameColor");
		backgroundColor = serializedObject.FindProperty("backgroundColor");
		comment = serializedObject.FindProperty("comment");
		commentColor = serializedObject.FindProperty("commentColor");
	}
	
	public override void OnInspectorGUI() {
		serializedObject.Update();

		EditorGUILayout.Space();
		GUIStyle style = new GUIStyle();

		//Preview
		GUI.backgroundColor = backgroundColor.colorValue;
		style.normal.background = EditorGUIUtility.whiteTexture;
		if(textColor.colorValue.a != 0)
			style.normal.textColor = textColor.colorValue;
		EditorGUILayout.LabelField(serializedObject.targetObject.name, style);
		GUI.backgroundColor = Color.white;

		//Colors
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 20;
		EditorGUILayout.PropertyField(textColor, new GUIContent("Aa"));
		EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Bg"));
		EditorGUIUtility.labelWidth = 0;
		EditorGUILayout.EndHorizontal();

		//Comment
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(comment);
		EditorGUILayout.PropertyField(commentColor, new GUIContent(""), GUILayout.Width(70));
		EditorGUILayout.EndHorizontal();
		
		serializedObject.ApplyModifiedProperties();
	}

	static void HierarchyWindowItem(int selectionID, Rect selectionRect) {
		GameObject o = (GameObject)EditorUtility.InstanceIDToObject(selectionID);
		if (!o) return;

		HierarchyStyle target = o.GetComponent<HierarchyStyle>();
		
		if (target && Event.current.type == EventType.Repaint) {
			GUIStyle style = new GUIStyle();
			style.alignment = TextAnchor.MiddleLeft;

			if (!Selection.Contains(o)) {
				//Redraw
				if (target.nameColor.a != 0) {
					Color editorBgColor = EditorGUIUtility.isProSkin
								 ? new Color32(56, 56, 56, 255)
								 : new Color32(194, 194, 194, 255);
					Rect rect = selectionRect;
					rect.width = style.CalcSize(new GUIContent(target.name)).x;
					EditorGUI.DrawRect(rect, editorBgColor);
				}

				//Background
				EditorGUI.DrawRect(selectionRect, target.backgroundColor);

				//Name
				if (target.nameColor.a == 0 && target.backgroundColor.a != 0) {
					style.normal.textColor = EditorGUIUtility.isProSkin
											 ? new Color(1, 1, 1, .5f)
											 : Color.black;
				} else
					style.normal.textColor = target.nameColor;

				if (!o.activeInHierarchy) {
					if (EditorGUIUtility.isProSkin)
						style.normal.textColor *= .7f;
					else
						style.normal.textColor -= new Color(0, 0, 0, .4f);
				}
				if (target.nameColor.a != 0 || target.backgroundColor.a != 0)
					style.Draw(selectionRect, new GUIContent(o.name), selectionID);
			}

			//Comment
			style.normal.textColor = target.commentColor;
			Rect commentRect = selectionRect;
			commentRect.width = style.CalcSize(new GUIContent(target.comment)).x;

			commentRect.x = EditorGUIUtility.currentViewWidth;
			commentRect.x -= commentRect.width + 20;

			style.Draw(commentRect, new GUIContent(target.comment), selectionID);
			EditorApplication.RepaintHierarchyWindow();
		}
	}
}
