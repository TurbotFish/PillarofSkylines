using UnityEditor;
using UnityEngine;

namespace grreuze {

	[CustomPropertyDrawer(typeof(DIOModelAsset))]
	public class DIOModelDrawer : PropertyDrawer {

		#region Popup Options

		string[] meshCompressionOptions = { "Off", "Low", "Medium", "High" };

		string[] normalsOptions = { "Import", "Calculate", "None" };

		string[] normalsModeOptions = { "Unweighted Legacy", "Unweighted", "Area Weighted", "Angle Weighted", "Area and Angle Weighted" };

		string[] tangentOptions = { "Import", "Calculate Tangent Space", "Calculate Legacy", "Calculate Legacy - Split Tangents", "None" };

		string[] matNameOptions = { "By Base Texture Name", "From Model's Material", "Model Name + Model's Material" };

		string[] matSearchOptions = { "Local Materials Folder", "Recursive-Up", "Project-Wide" };

		string[] animOptions = { "None", "Legacy", "Generic", "Humanoid" };
		#endregion

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, label, property);
			{
				EditorGUILayout.LabelField("Model Asset Import Options", EditorStyles.boldLabel, GUILayout.Height(20));

				EditorGUILayout.PropertyField(property.FindPropertyRelative("name"), GUIContent.none);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField("Name Contains", GUILayout.Width(100));
				EditorGUILayout.PropertyField(property.FindPropertyRelative("prefix"), GUIContent.none, GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				DoMeshes(property);

				EditorGUILayout.Space();

				DoNormalsAndTangents(property);

				EditorGUILayout.Space();

				DoMaterials(property);

				EditorGUILayout.Space();

				DoAnimation(property);

				EditorGUILayout.Space();

				DoTransform(property);
			}
			EditorGUI.EndProperty();
		}

		#region Sections

		void DoMeshes(SerializedProperty property) {

			EditorGUILayout.LabelField("Meshes", EditorStyles.boldLabel, GUILayout.Height(20));

			EditorGUILayout.PropertyField(property.FindPropertyRelative("scaleFactor"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("useFileScale"));

			SerializedProperty meshCompression = property.FindPropertyRelative("meshCompression");
			meshCompression.intValue = DrawPopup("Mesh Compression", meshCompression.intValue, meshCompressionOptions);

			EditorGUILayout.PropertyField(property.FindPropertyRelative("readWriteEnabled"), new GUIContent("Read/Write Enabled"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("optimizeMesh"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("importBlendShapes"), new GUIContent("Import BlendShapes"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("generateColliders"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("keepQuads"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("weldVertices"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("importVisibility"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("importCameras"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("importLights"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("swapUVs"), new GUIContent("Swap UVs"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("generateLightmapUVs"), new GUIContent("Generate Lightmap UVs"));
		}

		void DoNormalsAndTangents(SerializedProperty property) {

			EditorGUILayout.LabelField("Normals & Tangents", EditorStyles.boldLabel, GUILayout.Height(20));

			SerializedProperty normals = property.FindPropertyRelative("meshCompression");
			normals.intValue = DrawPopup("Normals", normals.intValue, normalsOptions);

			EditorGUI.BeginDisabledGroup(normals.intValue != 1);

			SerializedProperty normalsMode = property.FindPropertyRelative("normalsMode");
			normalsMode.intValue = DrawPopup("Normals Mode", normalsMode.intValue, normalsModeOptions);

			EditorGUILayout.PropertyField(property.FindPropertyRelative("smoothingAngle"));

			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(normals.intValue == 2);

			SerializedProperty tangents = property.FindPropertyRelative("tangents");
			int tangentSelected = TangentCodeToEditor(tangents.intValue);
			tangentSelected = DrawPopup("Normals Mode", tangentSelected, tangentOptions);
			tangents.intValue = TangentEditorToCode(tangentSelected);

			EditorGUI.EndDisabledGroup();
		}

		void DoMaterials(SerializedProperty property) {

			EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel, GUILayout.Height(20));

			SerializedProperty importMaterials = property.FindPropertyRelative("importMaterials");
			importMaterials.boolValue = EditorGUILayout.Toggle(new GUIContent("Import Materials"), importMaterials.boolValue);

			if (importMaterials.boolValue) {

				SerializedProperty materialNaming = property.FindPropertyRelative("materialNaming");
				materialNaming.intValue = DrawPopup("Material Naming", materialNaming.intValue, matNameOptions);

				SerializedProperty materialSearch = property.FindPropertyRelative("materialSearch");
				materialSearch.intValue = DrawPopup("Material Search", materialSearch.intValue, matSearchOptions);
			}
		}

		void DoAnimation(SerializedProperty property) {

			EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel, GUILayout.Height(20));

			EditorGUILayout.PropertyField(property.FindPropertyRelative("importAnimation"));

			SerializedProperty animationType = property.FindPropertyRelative("animationType");
			animationType.intValue = DrawPopup("Animation Type", animationType.intValue, animOptions);
		}

		void DoTransform(SerializedProperty property) {

			EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel, GUILayout.Height(20));

			EditorGUILayout.PropertyField(property.FindPropertyRelative("resetPosition"));
			EditorGUILayout.PropertyField(property.FindPropertyRelative("resetRotation"));
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

		int TangentCodeToEditor(int a) {
			switch (a) {
				case 0: return 0;
				case 1: return 2;
				case 2: return 4;
				case 3: return 1;
				case 4: return 3;
				default: return 1;
			}
		}

		int TangentEditorToCode(int a) {
			switch (a) {
				case 0: return 0;
				case 1: return 3;
				case 2: return 1;
				case 3: return 4;
				case 4: return 2;
				default: return 3;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return 0f;
		}
		#endregion
	}
}