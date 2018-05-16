using UnityEditor;
using UnityEngine;

namespace grreuze {

	public class DIOWindow : EditorWindow {

		Vector2 scrollPos, scrollPosUnder;
		bool editing;
		int listIndex, index;

		float currentScrollViewHeight;
		bool resize = false;
		Rect cursorChangeRect;

		Texture2D separator;

		[SerializeField] DIOTextureAsset[] _Textures;
		[SerializeField] DIOModelAsset[] _Models;

		void OnEnable() {
			DIOData.LoadData();
			_Textures = DIOData.tex;
			_Models = DIOData.model;

			currentScrollViewHeight = position.height / 2;
			cursorChangeRect = new Rect(0, currentScrollViewHeight, position.width, 1f);

            separator = new Texture2D(1, 1);
            separator.SetPixel(0, 0, new Color(0, 0, 0, .5f));
			separator.Apply();
		}

		[MenuItem("Assets/Default Import Options", false, 50)]
		public static void ShowWindow() {
			GetWindow(typeof(DIOWindow), false, "Default Import Options");
			GetWindow<DIOWindow>().Show();
		}

		void OnGUI() {
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Default Import Options:", EditorStyles.boldLabel);

			SerializedObject sO = new SerializedObject(this);
			SerializedProperty tex = sO.FindProperty("_Textures");
			SerializedProperty model = sO.FindProperty("_Models");

			sO.Update();

			GUILayout.BeginVertical();

			EditorGUILayout.Space();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(editing ? currentScrollViewHeight - 30 : position.height - 100));

			DrawList(tex, 0);
			EditorGUILayout.Space();
			DrawList(model, 1);

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndScrollView();

			// IF EDITING
			if (editing) {
				ResizeScrollView();

				EditorGUILayout.Space();
				//EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				scrollPosUnder = EditorGUILayout.BeginScrollView(scrollPosUnder);

				if (listIndex == 0)
					EditorGUILayout.PropertyField(tex.GetArrayElementAtIndex(index), true);
				else if (listIndex == 1)
					EditorGUILayout.PropertyField(model.GetArrayElementAtIndex(index), true);

				EditorGUILayout.EndScrollView();
			}

			GUILayout.FlexibleSpace();

			sO.ApplyModifiedProperties();

			EditorGUILayout.Space();
			if (GUILayout.Button("Apply Changes")) {
				DIOData.tex = _Textures;
				DIOData.model = _Models;
				DIOData.SaveData();
				editing = false;
			}
			if (GUILayout.Button("Reset All", EditorStyles.miniButton)) {
				DIOData.Initialize();
				_Textures = DIOData.tex;
				_Models = DIOData.model;
				DIOData.SaveData();
			}

			GUILayout.EndVertical();
			Repaint();
		}

		void DrawList(SerializedProperty list, int listID) {
			EditorGUILayout.LabelField(list.displayName, EditorStyles.boldLabel, GUILayout.Height(20));

			EditorGUI.indentLevel++;
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(list.GetArrayElementAtIndex(i).displayName, GUILayout.Width(130f));
				EditorGUI.BeginDisabledGroup(editing && listIndex == listID && index == i);
				if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.MaxWidth(100f))) {
					editing = true;
					listIndex = listID;
					index = i;
				}
				EditorGUI.EndDisabledGroup();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("v"), EditorStyles.miniButtonLeft, GUILayout.Width(20f))) {
					list.MoveArrayElement(i, i + 1);
				}
				if (GUILayout.Button(new GUIContent("^"), EditorStyles.miniButtonRight, GUILayout.Width(20f))) {
					list.MoveArrayElement(i, i - 1);
				}
				if (GUILayout.Button(new GUIContent("+"), EditorStyles.miniButtonLeft, GUILayout.Width(20f))) {
					list.InsertArrayElementAtIndex(i);
				}
				if (GUILayout.Button(new GUIContent("x"), EditorStyles.miniButtonRight, GUILayout.Width(20f))) {
					list.DeleteArrayElementAtIndex(i);
				}

				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Add New"), EditorStyles.miniButton, GUILayout.MaxWidth(84f))) {
				list.InsertArrayElementAtIndex(list.arraySize - 1);
			}
			EditorGUILayout.EndHorizontal();
		}

		void ResizeScrollView() {
			cursorChangeRect.height = 1;
			cursorChangeRect.width = position.width;
			GUI.DrawTexture(cursorChangeRect, separator);
			cursorChangeRect.height = 5;
			EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

			if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition)) {
				resize = true;
			}
			if (resize) {
				currentScrollViewHeight = Event.current.mousePosition.y;
				cursorChangeRect.Set(cursorChangeRect.x, currentScrollViewHeight, position.width, cursorChangeRect.height);
			}
			if (Event.current.type == EventType.MouseUp)
				resize = false;
		}

	}
}
