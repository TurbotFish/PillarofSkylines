using UnityEditor;
using UnityEngine;

namespace grreuze {

	[CustomPropertyDrawer(typeof(DIOTextureAsset))]
	public class DIOTextureDrawer : PropertyDrawer {

		#region Popup Options

		string[] textureTypeOptions = { "Default", "Normal Map", "Editor GUI and Legacy GUI",
										"Sprite (2D and UI)", "Cursor", "Cookie",
										"Lightmap", "Single Channel" };

		string[] alphaSourceOptions = { "None", "Input Texture Alpha", "From Gray Scale" };

		string[] normalFilterOptions = { "Sharp", "Smooth" };

		string[] spriteModeOptions = { "Single", "Multiple", "Polygon" };

		string[] spriteMeshOptions = { "Full Rect", "Tight" };

		string[] pivotOptions = { "Center", "Top Left", "Top", "Top Right", "Left", "Right", "Bottom Left", "Bottom", "Bottom Right", "Custom" };

		string[] npotOptions = { "None", "ToNearest", "ToLarger", "ToSmaller" };

		string[] mipMapOptions = { "Box", "Kaiser" };

		string[] wrapOptions = { "Repeat", "Clamp", "Mirror", "Mirror Once", "Per-Axis" };

		string[] filterOptions = { "Point (no filter)", "Bilinear", "Trilinear" };

		#endregion

		bool advanced = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, label, property);
			{
				EditorGUILayout.LabelField("Texture Asset Import Options", EditorStyles.boldLabel, GUILayout.Height(20));

				EditorGUILayout.PropertyField(property.FindPropertyRelative("name"), GUIContent.none);

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField("Name Contains", GUILayout.Width(100));
				EditorGUILayout.PropertyField(property.FindPropertyRelative("prefix"), GUIContent.none, GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				// Texture Type
				SerializedProperty textureType = property.FindPropertyRelative("textureType");
				int typeSelected = TextureTypeCodeToEditor(textureType.intValue);
				typeSelected = DrawPopup("Texture Type", typeSelected, textureTypeOptions);
				textureType.intValue = TextureTypeEditorToCode(typeSelected);

				EditorGUILayout.Space();

				if (typeSelected == 0)
					EditorGUILayout.PropertyField(property.FindPropertyRelative("sRGB"), new GUIContent("sRGB (Color Texture)"));

				if (typeSelected == 0 || typeSelected == 5 || typeSelected == 7)
					DoAlpha(property);

				if (typeSelected == 1)
					DoNormal(property);

				if (typeSelected == 3)
					DoSprite(property);

				EditorGUILayout.Space();

				DoAdvanced(property, typeSelected);

				EditorGUILayout.Space();


				SerializedProperty wrapMode = property.FindPropertyRelative("wrapMode");
				wrapMode.intValue = DrawPopup("Wrap Mode", wrapMode.intValue, wrapOptions);
				if (wrapMode.intValue == 4) {
					EditorGUI.indentLevel++;

					SerializedProperty wrapU = property.FindPropertyRelative("wrapModeU");
					wrapU.intValue = DrawPopup("U axis", wrapU.intValue, wrapOptions);

					SerializedProperty wrapV = property.FindPropertyRelative("wrapModeV");
					wrapV.intValue = DrawPopup("V axis", wrapV.intValue, wrapOptions);

					EditorGUI.indentLevel--;
				}

				SerializedProperty filterMode = property.FindPropertyRelative("filterMode");
				filterMode.intValue = DrawPopup("Filter Mode", filterMode.intValue, filterOptions);

				EditorGUILayout.PropertyField(property.FindPropertyRelative("anisoLevel"));
			}
			EditorGUI.EndProperty();
		}

		#region Sections

		void DoAlpha(SerializedProperty property) {
			SerializedProperty alphaSource = property.FindPropertyRelative("alphaSource");
			alphaSource.intValue = DrawPopup("Alpha Source", alphaSource.intValue, alphaSourceOptions);
			EditorGUI.BeginDisabledGroup(alphaSource.intValue == 0);
			EditorGUILayout.PropertyField(property.FindPropertyRelative("alphaIsTransparency"));
			EditorGUI.EndDisabledGroup();
		}

		void DoNormal(SerializedProperty property) {

			SerializedProperty createFromGrayscale = property.FindPropertyRelative("createFromGrayscale");
			EditorGUILayout.PropertyField(createFromGrayscale);

			if (createFromGrayscale.boolValue) {
				EditorGUI.indentLevel++;
				// Bumpiness
				EditorGUILayout.PropertyField(property.FindPropertyRelative("bumpiness"));

				// Normal Filtering
				SerializedProperty normalFilter = property.FindPropertyRelative("normalMapFilter");
				normalFilter.intValue = DrawPopup("Filtering", normalFilter.intValue, normalFilterOptions);

				EditorGUI.indentLevel--;
			}
		}

		void DoSprite(SerializedProperty property) {

			SerializedProperty spriteMode = property.FindPropertyRelative("spriteMode");
			if (spriteMode.intValue == 0) spriteMode.intValue = 1;
			int modeSelected = spriteMode.intValue - 1;
			modeSelected = DrawPopup("Sprite Mode", modeSelected, spriteModeOptions);
			spriteMode.intValue = modeSelected + 1;

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(property.FindPropertyRelative("pixelsPerUnit"));

			SerializedProperty meshType = property.FindPropertyRelative("meshType");
			meshType.intValue = DrawPopup("Mesh Type", meshType.intValue, spriteMeshOptions);

			EditorGUILayout.PropertyField(property.FindPropertyRelative("extrudeEdges"));

			SerializedProperty pivot = property.FindPropertyRelative("spriteAlignment");
			pivot.intValue = DrawPopup("Pivot", pivot.intValue, pivotOptions);
			if (pivot.intValue == 9) {
				EditorGUILayout.BeginHorizontal();

				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 50;
				EditorGUILayout.LabelField(new GUIContent(" "), GUILayout.Width(132));
				EditorGUILayout.PropertyField(property.FindPropertyRelative("pivotX"), new GUIContent("X"));
				EditorGUILayout.PropertyField(property.FindPropertyRelative("pivotY"), new GUIContent("Y"));
				EditorGUIUtility.labelWidth = labelWidth;

				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel--;
		}

		void DoAdvanced(SerializedProperty property, int typeSelected) {
			advanced = EditorGUILayout.Foldout(advanced, new GUIContent("Advanced"), true);
			if (advanced) {
				EditorGUI.indentLevel++;

				if (typeSelected == 3)
					EditorGUILayout.PropertyField(property.FindPropertyRelative("sRGB"), new GUIContent("sRGB (Color Texture)"));

				if (typeSelected > 1 && typeSelected < 5)
					DoAlpha(property);

				SerializedProperty npot = property.FindPropertyRelative("nonPowerOf2");
				npot.intValue = DrawPopup("Non Power of 2", npot.intValue, npotOptions);
				EditorGUILayout.PropertyField(property.FindPropertyRelative("readWriteEnabled"), new GUIContent("Read/Write Enabled"));

				DoMipMaps(property);

				EditorGUI.indentLevel--;
			}
		}

		void DoMipMaps(SerializedProperty property) {
			SerializedProperty mipMaps = property.FindPropertyRelative("generateMipMaps");
			EditorGUILayout.PropertyField(mipMaps);
			if (mipMaps.boolValue) {
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(property.FindPropertyRelative("borderMipMaps"));

				SerializedProperty mipMapFiltering = property.FindPropertyRelative("mipMapFiltering");
				mipMapFiltering.intValue = DrawPopup("Mip Map Filtering", mipMapFiltering.intValue, mipMapOptions);

				SerializedProperty coverage = property.FindPropertyRelative("mipMapPreserveCoverage");
				EditorGUILayout.PropertyField(coverage);
				if (coverage.boolValue) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(property.FindPropertyRelative("alphaCutoff"), new GUIContent("Alpha Cutoff Value"));
					EditorGUI.indentLevel--;
				}
				SerializedProperty fadeOut = property.FindPropertyRelative("fadeoutMipMaps");
				EditorGUILayout.PropertyField(fadeOut);
				if (fadeOut.boolValue) {
					EditorGUI.indentLevel++;

					SerializedProperty fadeStart = property.FindPropertyRelative("mipMapFadeStart");
					SerializedProperty fadeEnd = property.FindPropertyRelative("mipMapFadeEnd");
					float fadeStartf = fadeStart.intValue;
					float fadeEndf = fadeEnd.intValue;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Fade Range"), GUILayout.MaxWidth(132));
					EditorGUILayout.MinMaxSlider(ref fadeStartf, ref fadeEndf, 0, 10);
					EditorGUILayout.EndHorizontal();

					fadeStart.intValue = Mathf.RoundToInt(fadeStartf);
					fadeEnd.intValue = Mathf.RoundToInt(fadeEndf);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
		}

		#endregion

		#region Utilities

		int DrawPopup(string label, int index, string[] options) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent(label), GUILayout.MaxWidth(132));
			index = EditorGUILayout.Popup(index, options);
			EditorGUILayout.EndHorizontal();
			return index;
		}

		int TextureTypeEditorToCode(int a) {
			switch (a) {
				case 0:
				case 1:
				case 2:
				case 6:
					return a;
				case 3: return 8;
				case 4: return 7;
				case 5: return 4;
				case 7: return 10;
				default: return 0;
			}
		}

		int TextureTypeCodeToEditor(int a) {
			switch (a) {
				case 0:
				case 1:
				case 2:
				case 6:
					return a;
				case 8: return 3;
				case 7: return 4;
				case 4: return 5;
				case 10: return 7;
				default: return 0;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return 0f;
		}
		#endregion
	}
}